import { useLiveReadings } from '../hooks/useLiveReadings'

export function LiveReadingsPanel() {
  const { status, readings } = useLiveReadings()

  return (
    <div className="p-4 border rounded-lg bg-white">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold">Live Readings</h2>
        <div>
          <span className={`px-2 py-1 rounded text-xs ${status === 'live' ? 'bg-green-100 text-green-800' : status === 'connecting' ? 'bg-yellow-100 text-yellow-800' : 'bg-red-100 text-red-800'}`}>
            {status === 'live' ? 'Live' : status === 'connecting' ? 'Connecting' : 'Disconnected'}
          </span>
        </div>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="text-left text-xs text-gray-600">
              <th className="pb-2">Time</th>
              <th className="pb-2">Device</th>
              <th className="pb-2">Sensor</th>
              <th className="pb-2">Value</th>
            </tr>
          </thead>
          <tbody>
            {readings.length === 0 && (
              <tr><td colSpan={4} className="text-gray-500 py-4">No live readings yet</td></tr>
            )}
            {readings.map(r => (
              <tr key={`${r.deviceId}-${r.sensorType}`} className="border-t">
                <td className="py-2 align-top">{new Date(r.timestamp).toLocaleString()}</td>
                <td className="py-2 align-top">{r.deviceName} (#{r.deviceId})</td>
                <td className="py-2 align-top">{r.sensorType}</td>
                <td className="py-2 align-top">{r.value} {r.unit}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
