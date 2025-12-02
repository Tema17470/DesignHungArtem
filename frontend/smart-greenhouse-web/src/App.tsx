import Dashboard from './pages/Dashboard'
import LiveDashboardPage from './pages/LiveDashboardPage'

export default function App() {
	const path = typeof window !== 'undefined' ? window.location.pathname : '/'
	if (path === '/live') return <LiveDashboardPage />
	return <Dashboard />
}