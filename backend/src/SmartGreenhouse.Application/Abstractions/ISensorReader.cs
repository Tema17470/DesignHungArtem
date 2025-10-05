using System.Threading;
using System.Threading.Tasks;
namespace Application.Abstractions;

public interface ISensorReader
{
    Task<double> ReadAsync(int deviceId, SensorTypeEnum sensorType, CancellationToken ct = default);
    string UnitFor(SensorTypeEnum sensorType);
}

