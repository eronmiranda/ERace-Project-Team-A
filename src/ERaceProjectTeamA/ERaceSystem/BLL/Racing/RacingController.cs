using ERaceSystem.DAL;
using ERaceSystem.VIEWMODELS.Racing;
using ERaceSystem.ENTITIES;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeCode.Exceptions;


namespace ERaceSystem.BLL.Racing
{
    [DataObject]
    public class RacingController
    {
        #region Registration
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<ScheduleViewModel> GetSchedulesByDate(DateTime date)
        {
            using (var context = new ERaceSystemContext()) {
                var records = from x in context.Races
                              where SqlFunctions.DatePart("year", x.RaceDate) == SqlFunctions.DatePart("year", date)
                              where SqlFunctions.DatePart("dayofyear", x.RaceDate) == SqlFunctions.DatePart("dayofyear", date)
                              orderby x.RaceDate
                              select new ScheduleViewModel
                              {
                                  RaceID = x.RaceID,
                                  Time = x.RaceDate,
                                  Competition = x.Certification.Description + " - " + x.Comment,
                                  Run = x.Run,
                                  Drivers = x.NumberOfCars
                              };
                return records.ToList();
            }
        }

        public bool RaceIsRun(int raceID)
        {
            using (var context = new ERaceSystemContext())
            {
                return context.Races.Where(x => x.RaceID == raceID).Select(x => x.Run).Single() == "Y";
            }
        }

        public string GetEmployeeName(int? employeeId)
        {
            using (var context = new ERaceSystemContext())
            {
                return context.Employees.Where(x => x.EmployeeID == employeeId).Select(x => x.FirstName + " " + x.LastName).SingleOrDefault();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<RosterViewModel> GetRosterByRaceID(int raceID)
        {
            using (var context = new ERaceSystemContext())
            {
                var roster = from x in context.RaceDetails
                             where x.RaceID == raceID
                             select new RosterViewModel
                             {
                                 RaceDetailID = x.RaceDetailID,
                                 MemberID = x.MemberID,

                                 Name = x.Member.FirstName + " " + x.Member.LastName,
                                 RaceFee = Math.Round(x.RaceFee, 2),
                                 RentalFee = Math.Round(x.RentalFee, 2),
                                 Placement = x.Place,
                                 Refunded = x.Refund,

                                 CarClassID = x.CarID == null ? 0 : x.Car.CarClassID,
                                 SerialNumber = x.Car.SerialNumber,
                                 Comment = x.Comment,
                                 Reason = x.RefundReason
                             };
                return roster.ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public void UpdateRoster(RosterViewModel roster)
        {
            using (var context = new ERaceSystemContext())
            {

                List<Exception> brokenRules = new List<Exception>();
                var raceDetail = context.RaceDetails.First(x => x.RaceDetailID == roster.RaceDetailID);
                raceDetail.Comment = roster.Comment;

                var race = context.Races.Where(x => x.RaceID == context.RaceDetails.Where(y => y.RaceDetailID == roster.RaceDetailID).Select(y => y.RaceID).FirstOrDefault()).FirstOrDefault();
                if (race.Run == "Y")
                {
                    brokenRules.Add(new Exception("This Race has already been Run. Changes are no longer allowed."));
                }

                // If the roster was refunded
                if (roster.Refunded)
                {
                    // If no reason
                    // Spec: Refund must be accompanied by a reason.
                    if (String.IsNullOrWhiteSpace(roster.Reason))
                    {
                        brokenRules.Add(new Exception("Reason must be given for refund."));
                    }
                    else
                    {
                        raceDetail.RentalFee = 0;
                        raceDetail.Refund = true;
                        raceDetail.RefundReason = roster.Reason;
                        raceDetail.CarID = null;
                        raceDetail.PenaltyID = context.RacePenalties.Where(x => x.Description == "Scratched").Select(x => x.PenaltyID).Single();
                    }
                }
                // Race not refunded.
                else
                {
                    raceDetail.Refund = false;
                    raceDetail.RefundReason = null;

                    if (roster.SerialNumber != null)
                    {
                        var car = context.Cars.Where(x => x.SerialNumber == roster.SerialNumber).FirstOrDefault();
                        var rentalFee = context.CarClasses.Where(x => x.CarClassID == car.CarClassID).Select(x => x.RaceRentalFee).FirstOrDefault();

                        // If duplicate Car
                        var duplicateCarInRace = context.RaceDetails.Where(x => x.CarID == car.CarID).Where(x => x.RaceDetailID != roster.RaceDetailID).Where(x => x.RaceID == race.RaceID).FirstOrDefault();
                        if (duplicateCarInRace != null && duplicateCarInRace.CarID != null)
                        {
                            brokenRules.Add(new Exception("Duplicate Cars are not allowed int the race."));
                        } 
                        else 
                        {

                            // Spec: Vehicles must fit into the matching certification level that is specified by the Races table.
                            var raceCertification = context.Races.Where(x => x.RaceID == roster.RaceID).Select(x => x.CertificationLevel).FirstOrDefault();
                            var carCertification = context.CarClasses.Where(x => x.CarClassID == car.CarClassID).Select(x => x.CertificationLevel).FirstOrDefault();

                            if (carCertification != raceCertification)
                            {
                                brokenRules.Add(new Exception("Vehicles must fit into the matching certifivation level taht is specified by the Races table."));
                            }
                            else
                            {
                                raceDetail.CarID = car.CarID;
                                raceDetail.RentalFee = Math.Round(rentalFee, 2);
                            }
                        }
                    }
                    else
                    {
                        // No car and race day
                        if (DateTime.Today == race.RaceDate.Date)
                        {
                            brokenRules.Add(new Exception("A valid rental car is required to enter the race."));
                        }
                    }
                }
                
                
                
                
                
                if (brokenRules.Any())
                {
                    throw new BusinessRuleCollectionException("Update Roster BLLExceptions", brokenRules);
                } 
                else 
                {
                    // Spec: All Processing done as a single transaction.
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        context.SaveChanges();
                        transaction.Commit();
                        transaction.Dispose();
                    }
                }
            }
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public void AddRoster(RosterViewModel roster)
        {
            // Spec: All processing must be done as a single transaction.
            using (var context = new ERaceSystemContext())
            {
                List<Exception> brokenRules = new List<Exception>();

                var race = context.Races.Where(x => x.RaceID == roster.RaceID).FirstOrDefault();

                //Business Rules
                if (race.Run == "Y")
                {
                    brokenRules.Add(new Exception("This Race has already been Run. Changes are no longer allowed."));
                }


                // Spec: The race fee is payable at this time and must be recorded as part of the race details.
                var raceDetail = new RaceDetail
                        {
                            RaceID = roster.RaceID,
                            RaceFee = Math.Round(roster.RaceFee, 2)
                        };

                // TODO Check count.
                if (context.RaceDetails.Where(x => x.RaceID == race.RaceID).Count() >= race.NumberOfCars)
                {
                    brokenRules.Add(new Exception("This race is already full"));
                }

                //Member double book
                // Spec: Members cannot be double registerd in a given race.
                var duplicateDriver = context.RaceDetails.Where(x => x.RaceID == roster.RaceID).Where(x => x.MemberID == roster.MemberID).FirstOrDefault();
                if (duplicateDriver != null)
                {
                    brokenRules.Add(new Exception("Members cannot be double-registered in a given race."));
                }
                else
                {
                    raceDetail.MemberID = roster.MemberID;
                }


                // Spec: Vehicle rantal can be done at this time but drives can renta at a later time.
                // Spec: Vehicles must fit into the matching certification level that is specified by the Races table.
                if (!String.IsNullOrWhiteSpace(roster.SerialNumber) && roster.SerialNumber != "0" && roster.SerialNumber != "Select a Car")
                {
                    var car = context.Cars.Where(x => x.SerialNumber == roster.SerialNumber).FirstOrDefault();
                    var raceCertification = context.Races.Where(x => x.RaceID == roster.RaceID).Select(x => x.CertificationLevel).FirstOrDefault();
                    var carCertification = context.CarClasses.Where(x => x.CarClassID == car.CarClassID).Select(x => x.CertificationLevel).FirstOrDefault();

                    if (carCertification != raceCertification)
                    {
                        brokenRules.Add(new Exception("Vehicles must fit into the matching certifivation level taht is specified by the Races table."));
                    }
                    else
                    {
                        var rentalFee = context.CarClasses.Where(x => x.CarClassID == car.CarClassID).Select(x => x.RaceRentalFee).SingleOrDefault();

                        raceDetail.CarID = car.CarID;
                        raceDetail.RentalFee = Math.Round(rentalFee, 2);
                    }
                }

                var employeeID = roster.EmployeeID ?? 0;

                if (employeeID == 0 || !context.Employees.Where(x => x.EmployeeID == employeeID).Any())
                {
                    brokenRules.Add(new Exception("Only valid and permitted employees may enter data."));
                }


                if (brokenRules.Any())
                {
                    throw new BusinessRuleCollectionException("Add to Roster BLLExceptions", brokenRules);
                }
                else
                {

                    // Spec: All processing must be done as a single transaction.
                    using (var transaction = context.Database.BeginTransaction())
                    {

                        
                        // Spec: An invoice must be generated as part of the registration.
                        //Invoice
                        var invoice = new Invoice
                        {
                            InvoiceDate = DateTime.Now,
                            EmployeeID = employeeID,
                            SubTotal = Math.Round(roster.RaceFee),
                            GST = 5,
                        };

                        context.Invoices.Add(invoice);
                        context.SaveChanges();


                        //RaceDetails
                        raceDetail.InvoiceID = invoice.InvoiceID;
                        
                        context.RaceDetails.Add(raceDetail);
                        context.SaveChanges();

                        transaction.Commit();
                        transaction.Dispose();
                    }
                }
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<DriverViewModel> GetDriversNotInRaceByRaceID(int raceID)
        {
            using (var context = new ERaceSystemContext())
            {
                var members = from x in context.Members
                              where !(from y in context.RaceDetails
                                      where y.RaceID == raceID
                                      select y.MemberID).Contains(x.MemberID)
                              where x.CertificationLevel == (from z in context.Races
                                                             where z.RaceID == raceID
                                                             select z.CertificationLevel).FirstOrDefault()
                              select new DriverViewModel
                              {
                                  MemberID = x.MemberID,
                                  Name = x.FirstName + " " + x.LastName
                              };
                return members.ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<Decimal> GetRaceFees()
        {
            using (var context = new ERaceSystemContext())
            {
                var fees = from x in context.RaceFees
                           select Math.Round(x.Fee, 2);
                return fees.ToList();

            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CarClassViewModel> GetCarClassesFromRaceID(int raceID)
        {
            using (var context = new ERaceSystemContext())
            {
                var classes = from x in context.CarClasses
                              where x.CertificationLevel == context.Races.Where(y => y.RaceID == raceID).Select(y => y.CertificationLevel).FirstOrDefault()
                              select new CarClassViewModel
                              {
                                  ClassID = x.CarClassID,
                                  ClassName = x.CarClassName
                              };
                var list = classes.ToList();
                list.Insert(0, new CarClassViewModel { ClassID = 0, ClassName = "N/A" });
                return list;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CarViewModel> GetCarsFromClassID(int carClassID, int raceID)
        {
            using (var context = new ERaceSystemContext())
            {
                var cars = from x in context.Cars
                           where x.CarClassID == carClassID
                           where !(from y in context.RaceDetails
                                   where y.RaceID == raceID
                                   select y.CarID).Contains(x.CarID)
                           select new CarViewModel
                           {
                               CarID = x.CarID,
                               SerialNumber = x.SerialNumber
                           };
                var list = cars.ToList();
                list.Insert(0, new CarViewModel { CarID = 0, SerialNumber = "Select a Car" });
                return list;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<CarViewModel> GetCarsFromClassIDPlusCurrentCar(int carClassID, int raceID, int raceDetailID)
        {
            using (var context = new ERaceSystemContext())
            {
                var freeCars = GetCarsFromClassID(carClassID, raceID);

                var raceDetail = context.RaceDetails.Where(x => x.RaceDetailID == raceDetailID).Single();

                if (raceDetail.CarID != null)
                {

                    var currentCar = from x in context.Cars
                                     where x.CarID == raceDetail.CarID
                                     where x.CarClassID == carClassID
                                     select new CarViewModel
                                     {
                                         CarID = x.CarID,
                                         SerialNumber = x.SerialNumber
                                     };
                    if (currentCar.SingleOrDefault() != null)
                    {
                        freeCars.Insert(0, currentCar.Single());
                    }
                }

                return freeCars;
            }
        }


        public RosterViewModel GetRosterByRaceDetailID(int raceDetailID)
        {
            using (var context = new ERaceSystemContext())
            {
                var roster = from x in context.RaceDetails
                             where x.RaceDetailID == raceDetailID
                             select new RosterViewModel
                             {
                                 RaceDetailID = x.RaceDetailID,
                                 MemberID = x.MemberID,

                                 Name = x.Member.FirstName + " " + x.Member.LastName,
                                 RaceFee = Math.Round(x.RaceFee),
                                 RentalFee = Math.Round(x.RentalFee),
                                 Placement = x.Place,
                                 Refunded = x.Refund,

                                 CarClassID = x.Car.CarClassID,
                                 SerialNumber = x.Car.SerialNumber,
                                 Comment = x.Comment,
                                 Reason = x.RefundReason
                             };
                return roster.FirstOrDefault();
            }
        }
        #endregion

        #region Results

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<ResultViewModel> GetResultsByRaceID(int raceID)
        {
            using (var context = new ERaceSystemContext())
            {
                var results = from x in context.RaceDetails
                              where x.RaceID == raceID
                              let time = x.RunTime
                              select new ResultViewModel
                              {
                                  RaceDetailID = x.RaceDetailID,
                                  Name = x.Member.FirstName + " " + x.Member.LastName,
                                  Time = x.RunTime ?? TimeSpan.Zero,
                                  Penalty = x.PenaltyID == null ? 0 : x.PenaltyID,
                                  Placement = x.Place
                              };
                return results.ToList();
            }
        }
        
        [DataObjectMethod(DataObjectMethodType.Update, false)]
        public void UpdateResults(List<ResultViewModel> results)
        {
            using (var context = new ERaceSystemContext())
            {
                //Specs: All processing must be done as a single transaction

                // List of Errors.
                List<Exception> brokenRules = new List<Exception>();

                // Check that there are participants if so set the race to run.
                if (results.Count == 0)
                {
                    brokenRules.Add(new Exception("This race has no racers."));
                }
                else
                {
                    var firstResult = results[0];
                    var firstRaceDetail = context.RaceDetails.Where(x => x.RaceDetailID == firstResult.RaceDetailID).Single();
                    var race = context.Races.Where(x => x.RaceID == firstRaceDetail.RaceID).Single();
                    race.Run = "Y";
                }

                // Spec: Race Times Cannot be negative.
                // Check if any non-scractched racers are missing times.
                var invalidTimes = results.Where(x => x.Time <= TimeSpan.Zero).Where(x => x.Penalty != 4);
                if (invalidTimes.Any())
                {
                    brokenRules.Add(new Exception("All racers who competed must have a runtime entered."));
                }

                // Set the racer times. Sort by runtime and set position incrementally.
                results.Sort((x, y) => x.Time.CompareTo(y.Time));
                int place = 1;
                foreach (var result in results)
                {
                    // Sort by time
                    result.Time = (result.Time == null || result.Time == TimeSpan.Zero) ? TimeSpan.Zero : result.Time;
                    
                    // Give Places to those with places.
                    if (!(result.Penalty == 4))
                    {
                        // Spec: The drivers' places in the race must be calculated in the BLL and stored along with the race times.
                        result.Placement = place++;
                    } 
                    else
                    {
                        result.Placement = null;

                        // Spec: Drivers that have been scratched are given a race time of zero.
                        result.Time = TimeSpan.Zero;
                    }


                }

                // Check for errors or run.
                if (brokenRules.Any())
                {
                    throw new BusinessRuleCollectionException("Update Results BLLExceptions", brokenRules);
                }
                else
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        foreach (var result in results)
                        {
                            var raceDetail = context.RaceDetails.Where(x => x.RaceDetailID == result.RaceDetailID).Single();

                            // Spec: Penalties don't affect the driver's palce in the race; only the rae time that has been entered affects their placement. 
                            // Assume the times have been adjusted correctly by the Race Coordinator for penalties at the time they were entered by the user.
                            raceDetail.RunTime = result.Time;
                            raceDetail.PenaltyID = (result.Penalty == 0) ? null : result.Penalty;
                            raceDetail.Place = result.Placement;
                        }
                        context.SaveChanges();
                        

                        transaction.Commit();
                        transaction.Dispose();
                    }
                }
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<PenaltyViewModel> GetPenalties()
        {
            using (var context = new ERaceSystemContext())
            {
                var penalties = from x in context.RacePenalties
                                select new PenaltyViewModel
                                {
                                    PenaltyID = x.PenaltyID,
                                    Description = x.Description
                                };
                var list = penalties.ToList();
                list.Insert(0, new PenaltyViewModel { PenaltyID = 0, Description = "None" });
                return list;
            }
        }


        #endregion
    }
}
