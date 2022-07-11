Begin
	update Races
	set Run = 'N', RaceDate = DATEADD(month, 12 - month(RaceDate), DATEADD(year, 2020 - year(RaceDate), RaceDate));
End;
