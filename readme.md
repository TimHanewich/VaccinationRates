# VaccinationRates
A lightweight .NET class library for sourcing COVID-19 case and vaccination rates from Bing's COVID dashboard service.

## Installing into a .NET project
The package ID is `VaccinationRates` on Nuget. To install:
```
dotnet add package VaccinationRates
```

## Using the library
To request data:
```
using VaccinationRates;

VaccinationAreaData worldwidedata = await VaccinationAreaData.LoadWorldAsync();
```