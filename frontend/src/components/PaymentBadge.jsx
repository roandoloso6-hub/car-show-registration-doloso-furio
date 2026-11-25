const styles = {
  Unpaid: 'bg-red-50 text-red-700 ring-red-600/20',
  Partial: 'bg-amber-50 text-amber-700 ring-amber-600/20',
  Paid: 'bg-emerald-50 text-emerald-700 ring-emerald-600/20',
}

export default function PaymentBadge({ paymentStatus }) {
  return <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-semibold ring-1 ring-inset ${styles[paymentStatus] || 'bg-slate-100 text-slate-700 ring-slate-500/20'}`}>{paymentStatus}</span>
}
