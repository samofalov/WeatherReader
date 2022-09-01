using AwtSG.Domain.Geospatial.Gridded;
using AwtSG.Domain.MetOceanData;
using AwtSG.Domain.MetOceanData.Gridded;
using AwtSG.Domain.MetOceanData.Gridded.Interpolators;
using AwtSG.Domain.MetOceanData.Gridded.PackedBinaryFormat;
using AwtSG.Domain.NavigationUtilities;
using AwtSG.Infrastructure;
using AwtSG.Numerics.Interpolation;

// Set directory, time range
const string weatherFolder = @"\\no-dr-fs02\winapp\WeatherData\PBF";
Console.WriteLine($"Weather directory set to {weatherFolder}");

var loadFrom = new DateTime(2022, 08, 31, 17, 0, 0);
var loadTo = new DateTime(2022, 08, 31, 19, 0, 0);

var dateTimeRange = new Range<DateTime>(loadFrom, loadTo);
Console.WriteLine($"Loading weather data from {dateTimeRange.Minimum} to {dateTimeRange.Maximum}");

// Read files
var streamAccessorFactory = new PermanentStreamAccessorFactory();
var interpolator = new NearestValidNeighbor<float>();

var pbfDirectoryBasedFactory = new PbfWeatherDataIndexFactory(streamAccessorFactory, interpolator);
var weatherDataOnDisk = pbfDirectoryBasedFactory.IndexUngroupedData(weatherFolder, dateTimeRange, CancellationToken.None);
// weatherDataOnDisk.LoadDataIntoMemory().Wait(); // It is possible to load all weather in advance

// Create provider
var nearestNeighborInterpolator = new NearestNeighborInterpolator1D<float>();
var metOceanDataProvider = new GriddedMetOceanDataProvider(weatherDataOnDisk, new GriddedWeatherDataTimeInterpolatorSingle(nearestNeighborInterpolator, nearestNeighborInterpolator));

var position = new Position(35, 18);
var dateTime = new DateTime(2022, 08, 31, 18, 0, 0);
dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
var metOceanDataParameters = MetOceanDataParameters.All;

// Use provider to retrieve data
var data = metOceanDataProvider.GetMetOceanData(position, dateTime, metOceanDataParameters);

Console.Write(data.Wind.Speed);