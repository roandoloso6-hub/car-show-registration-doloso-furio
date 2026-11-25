const styles = {
  Pending: 'bg-amber-50 text-amber-700 ring-amber-600/20',
  Confirmed: 'bg-emerald-50 text-emerald-700 ring-emerald-600/20',
  CheckedIn: 'bg-teal-50 text-teal-700 ring-teal-600/20',
  Cancelled: 'bg-red-50 text-red-700 ring-red-600/20',
}

export default function StatusBadge({ status }) {
  return <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-semibold ring-1 ring-inset ${styles[status] || 'bg-slate-100 text-slate-700 ring-slate-500/20'}`}>{status}</span>
}
