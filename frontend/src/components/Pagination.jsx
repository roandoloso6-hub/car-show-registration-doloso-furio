export default function Pagination({ page, pageSize, totalCount, totalPages, loading, onPageChange, onPageSizeChange }) {
  const start = totalCount === 0 ? 0 : (page - 1) * pageSize + 1
  const end = Math.min(page * pageSize, totalCount)
  const pages = Array.from({ length: totalPages }, (_, i) => i + 1).filter((number) => number === 1 || number === totalPages || Math.abs(number - page) <= 1)

  return (
    <div className="flex flex-col gap-4 border-t border-slate-200 px-5 py-4 md:flex-row md:items-center md:justify-between">
      <p className="text-sm text-slate-500">Showing <span className="font-semibold text-slate-700">{start}–{end}</span> of <span className="font-semibold text-slate-700">{totalCount}</span></p>
      <div className="flex flex-wrap items-center gap-3">
        <label className="flex items-center gap-2 text-sm text-slate-500"><span>Rows</span><select value={pageSize} disabled={loading} onChange={(event) => onPageSizeChange(Number(event.target.value))} className="h-9 rounded-lg border border-slate-300 px-2 text-slate-700 outline-none transition focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/20"><option>5</option><option>10</option><option>20</option></select></label>
        <button type="button" disabled={loading || page <= 1} onClick={() => onPageChange(page - 1)} className="h-9 rounded-lg border border-slate-300 px-3 text-sm font-semibold text-slate-700 transition hover:border-emerald-200 hover:bg-emerald-50 hover:text-emerald-700 disabled:cursor-not-allowed disabled:opacity-40">Prev</button>
        {pages.map((number) => <button key={number} type="button" disabled={loading} onClick={() => onPageChange(number)} className={`h-9 min-w-9 rounded-lg px-2 text-sm font-semibold transition ${number === page ? 'bg-emerald-600 text-white hover:bg-emerald-700' : 'border border-slate-300 text-slate-700 hover:border-emerald-200 hover:bg-emerald-50 hover:text-emerald-700'}`}>{number}</button>)}
        <button type="button" disabled={loading || page >= totalPages} onClick={() => onPageChange(page + 1)} className="h-9 rounded-lg border border-slate-300 px-3 text-sm font-semibold text-slate-700 transition hover:border-emerald-200 hover:bg-emerald-50 hover:text-emerald-700 disabled:cursor-not-allowed disabled:opacity-40">Next</button>
      </div>
    </div>
  )
}
