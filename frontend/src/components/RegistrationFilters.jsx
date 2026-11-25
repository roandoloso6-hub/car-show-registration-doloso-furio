import useDebounce from '../hooks/useDebounce.js'
import { useEffect, useState } from 'react'

export default function RegistrationFilters({ filters, onChange, onReset }) {
  const [search, setSearch] = useState(filters.participantName)
  const debouncedSearch = useDebounce(search, 400)

  useEffect(() => {
    if (debouncedSearch !== filters.participantName) onChange({ participantName: debouncedSearch })
  }, [debouncedSearch, filters.participantName, onChange])

  const reset = () => {
    setSearch('')
    onReset()
  }

  const selectClass = "h-11 w-full appearance-none rounded-xl border border-slate-200 bg-slate-50 px-3.5 pr-9 text-sm font-medium text-slate-700 outline-none transition focus:border-emerald-500 focus:bg-white focus:ring-4 focus:ring-emerald-500/20"

  return (
    <section className="grid gap-3 rounded-2xl border border-slate-200/80 bg-white p-3.5 shadow-[0_1px_2px_rgba(15,23,42,0.02),0_8px_24px_rgba(15,23,42,0.04)] md:grid-cols-[minmax(280px,1fr)_170px_170px_auto]">
      <label className="relative block">
        <span className="sr-only">Search participant name</span>
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" className="pointer-events-none absolute left-3.5 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"><circle cx="11" cy="11" r="7"/><path d="m20 20-4-4"/></svg>
        <input
          value={search}
          onChange={(event) => setSearch(event.target.value)}
          placeholder="Search participant name..."
          className="h-11 w-full rounded-xl border border-slate-200 bg-slate-50 pl-10 pr-3 text-sm text-slate-900 outline-none transition placeholder:text-slate-400 focus:border-emerald-500 focus:bg-white focus:ring-4 focus:ring-emerald-500/20"
        />
      </label>

      <label className="relative block">
        <span className="sr-only">Type</span>
        <select value={filters.type} onChange={(event) => onChange({ type: event.target.value })} className={selectClass}>
          <option value="">All types</option><option>Exhibitor</option><option>Competitor</option><option>VIP</option>
        </select>
        <svg viewBox="0 0 20 20" fill="currentColor" className="pointer-events-none absolute right-3.5 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"><path d="M5.5 7.5 10 12l4.5-4.5H5.5Z"/></svg>
      </label>

      <label className="relative block">
        <span className="sr-only">Status</span>
        <select value={filters.status} onChange={(event) => onChange({ status: event.target.value })} className={selectClass}>
          <option value="">All statuses</option><option>Pending</option><option>Confirmed</option><option>CheckedIn</option><option>Cancelled</option>
        </select>
        <svg viewBox="0 0 20 20" fill="currentColor" className="pointer-events-none absolute right-3.5 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"><path d="M5.5 7.5 10 12l4.5-4.5H5.5Z"/></svg>
      </label>

      <button type="button" onClick={reset} className="h-11 rounded-xl border border-slate-200 px-4 text-sm font-bold text-slate-600 transition hover:border-emerald-200 hover:bg-emerald-50 hover:text-emerald-700">Reset</button>
    </section>
  )
}
