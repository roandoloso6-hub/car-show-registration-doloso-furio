import { useCallback, useEffect, useState } from 'react'
import * as registrationsApi from '../api/registrationsApi.js'
import KpiCards from '../components/KpiCards.jsx'
import RegistrationFilters from '../components/RegistrationFilters.jsx'
import RegistrationTable from '../components/RegistrationTable.jsx'
import Pagination from '../components/Pagination.jsx'

const dataSource = registrationsApi

export default function RegistrationsPage() {
  const [filters, setFilters] = useState({ participantName: '', type: '', status: '' })
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [data, setData] = useState({ items: [], page: 1, pageSize: 10, totalCount: 0, totalPages: 0 })
  const [summary, setSummary] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [retry, setRetry] = useState(0)

  const load = useCallback(async () => {
    setLoading(true)
    setError('')
    try {
      const [paged, totals] = await Promise.all([
        dataSource.getRegistrations({ ...filters, page, pageSize }),
        dataSource.getSummary(),
      ])
      setData(paged)
      setSummary(totals)
    } catch (requestError) {
      setError(requestError?.message || 'Cannot reach the API')
    } finally {
      setLoading(false)
    }
  }, [filters, page, pageSize, retry])

  useEffect(() => { load() }, [load])

  const changeFilter = (next) => {
    setFilters((current) => ({ ...current, ...next }))
    setPage(1)
  }

  const clearFilters = () => {
    setFilters({ participantName: '', type: '', status: '' })
    setPage(1)
  }

  return (
    <main className="min-h-screen bg-[#f4f7f5]">
      <header className="border-t-2 border-emerald-600 border-b border-emerald-100/80 bg-white/95 shadow-[0_1px_12px_rgba(6,95,70,0.04)]">
        <div className="mx-auto flex max-w-[1440px] items-center justify-between px-5 py-5 sm:px-8 lg:px-10">
          <div className="flex items-center gap-3">
            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-emerald-700 text-white shadow-lg shadow-emerald-900/15">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7" className="h-6 w-6"><path d="M5 17h14l-1.5-5.5A3 3 0 0 0 14.6 9H9.4a3 3 0 0 0-2.9 2.5L5 17Z"/><path d="M7 17v2M17 17v2M7.5 13h9"/><circle cx="8" cy="15" r="1"/><circle cx="16" cy="15" r="1"/></svg>
            </div>
            <div>
              <p className="text-[11px] font-bold uppercase tracking-[0.22em] text-emerald-700">AutoVista Events</p>
              <p className="mt-0.5 text-lg font-bold tracking-tight text-slate-950">Registration Management</p>
            </div>
          </div>
          <div className="hidden items-center gap-2 rounded-full border border-emerald-200 bg-emerald-50 px-3.5 py-2 text-xs font-bold text-emerald-700 sm:flex">
            <span className="h-2 w-2 rounded-full bg-emerald-500 shadow-[0_0_0_4px_rgba(16,185,129,0.10)]" />
            Car Show 2027
          </div>
        </div>
      </header>

      <div className="mx-auto max-w-[1440px] px-5 py-7 sm:px-8 lg:px-10 lg:py-9">
        <div className="mb-6 flex flex-col justify-between gap-3 sm:flex-row sm:items-end">
          <div>
            <p className="mb-1 text-xs font-bold uppercase tracking-[0.16em] text-emerald-700/70">Dashboard</p>
            <h1 className="text-3xl font-bold tracking-[-0.035em] text-slate-950 sm:text-[34px]">Car Show Registrations</h1>
          </div>
          <p className="text-sm text-slate-500">Monitor participants, payments, and event status.</p>
        </div>

        <div className="space-y-5">
          <KpiCards summary={summary} />
          <RegistrationFilters filters={filters} onChange={changeFilter} onReset={clearFilters} />

          <section className="overflow-hidden rounded-2xl border border-slate-200/80 bg-white shadow-[0_1px_2px_rgba(15,23,42,0.02),0_10px_35px_rgba(15,23,42,0.05)]">
            {loading && (
              <div className="flex items-center gap-3 border-b border-slate-100 px-5 py-3.5 text-sm font-semibold text-emerald-700">
                <span className="h-4 w-4 animate-spin rounded-full border-2 border-emerald-200 border-t-emerald-600" /> Loading registrations...
              </div>
            )}

            {error && (
              <div className="m-5 flex min-h-64 flex-col items-center justify-center rounded-xl border border-dashed border-red-200 bg-red-50/40 px-6 text-center">
                <div className="flex h-12 w-12 items-center justify-center rounded-full bg-red-100 text-red-600">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" className="h-6 w-6"><circle cx="12" cy="12" r="9"/><path d="M12 8v5M12 16h.01"/></svg>
                </div>
                <p className="mt-4 text-base font-bold text-slate-900">Cannot reach the API...</p>
                <p className="mt-1 max-w-md text-sm text-slate-500">Make sure the C# backend is running on the configured URL.</p>
                <button type="button" onClick={() => setRetry((value) => value + 1)} className="mt-5 rounded-lg bg-slate-900 px-4 py-2.5 text-sm font-bold text-white shadow-sm transition hover:bg-slate-700">Try again</button>
              </div>
            )}

            {!error && !loading && data.items.length === 0 && (
              <div className="flex min-h-72 flex-col items-center justify-center px-5 text-center">
                <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-slate-100 text-slate-400"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7" className="h-7 w-7"><circle cx="11" cy="11" r="7"/><path d="m20 20-4-4"/></svg></div>
                <p className="mt-4 text-base font-bold text-slate-900">No registrations match your filters</p>
                <p className="mt-1 text-sm text-slate-500">Try changing or clearing the current filters.</p>
                <button type="button" onClick={clearFilters} className="mt-5 rounded-lg border border-slate-300 bg-white px-4 py-2.5 text-sm font-bold text-slate-700 transition hover:bg-slate-50">Clear filters</button>
              </div>
            )}

            {!error && (loading || data.items.length > 0) && <RegistrationTable items={data.items} loading={loading} />}

            {!error && (
              <Pagination
                page={data.page || page}
                pageSize={pageSize}
                totalCount={data.totalCount}
                totalPages={data.totalPages}
                loading={loading}
                onPageChange={setPage}
                onPageSizeChange={(size) => { setPageSize(size); setPage(1) }}
              />
            )}
          </section>
        </div>
      </div>
    </main>
  )
}
