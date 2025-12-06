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
  const [list, setList] = useState<LiveReading[]>([])

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

          setList(prev => {
            const next = [...prev, obj]

            // keep only last 5 readings per device
            const grouped = new Map<string, LiveReading[]>()

            for (const r of next) {
              const key = `${r.deviceId}:${r.sensorType}`
              if (!grouped.has(key)) grouped.set(key, [])
              grouped.get(key)!.push(r)
            }

            // trim each group to last 5 by timestamp
            const trimmed: LiveReading[] = []
            for (const arr of grouped.values()) {
              arr.sort((a, b) => b.timestamp.localeCompare(a.timestamp))
              trimmed.push(...arr.slice(0, 5))
            }

            return trimmed
          })
        } catch (e) {
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

  const readings = [...list].sort(
    (a, b) => b.timestamp.localeCompare(a.timestamp)
  )

  return { status, readings }
}
