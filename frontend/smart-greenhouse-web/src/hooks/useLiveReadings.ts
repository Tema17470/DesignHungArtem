import { useEffect, useRef, useState } from 'react'

export type LiveReading = {
  id: number
  deviceId: number
  deviceName: string
  sensorType: string
  value: number
  unit: string
  timestamp: string
}

type Status = 'connecting' | 'live' | 'disconnected'

export function useLiveReadings(url = 'ws://localhost:5080/ws/live-readings') {
  const wsRef = useRef<WebSocket | null>(null)
  const [status, setStatus] = useState<Status>('connecting')
  const [map, setMap] = useState<Map<string, LiveReading>>(new Map())

  useEffect(() => {
    let cancelled = false

    function connect() {
      setStatus('connecting')
      const ws = new WebSocket(url)
      wsRef.current = ws

      ws.onopen = () => {
        if (cancelled) return
        setStatus('live')
      }

      ws.onmessage = evt => {
        try {
          const obj = JSON.parse(evt.data) as LiveReading
          // Key by device + sensor type to keep latest per sensor
          const key = `${obj.deviceId}:${obj.sensorType}`
          setMap(prev => {
            const copy = new Map(prev)
            // replace only if newer timestamp
            const cur = copy.get(key)
            if (!cur || (cur.timestamp ?? '') < (obj.timestamp ?? '')) {
              copy.set(key, obj)
            }
            return copy
          })
        } catch (e) {
          // ignore invalid messages
          console.warn('Invalid live message', e)
        }
      }

      ws.onclose = () => {
        if (cancelled) return
        setStatus('disconnected')
        // try reconnect after a short delay
        setTimeout(() => connect(), 2000)
      }

      ws.onerror = () => {
        // let onclose handle reconnect
      }
    }

    connect()

    return () => {
      cancelled = true
      try {
        wsRef.current?.close()
      } catch {}
    }
  }, [url])

  const readings = [...map.values()].sort((a, b) => (b.timestamp ?? '').localeCompare(a.timestamp ?? ''))

  return { status, readings }
}
