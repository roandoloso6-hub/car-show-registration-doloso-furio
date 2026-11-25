import StatusBadge from './StatusBadge.jsx'
import PaymentBadge from './PaymentBadge.jsx'

const peso = new Intl.NumberFormat('en-PH', { style: 'currency', currency: 'PHP', minimumFractionDigits: 2 })

function formatDate(value) {
  if (!value) return '—'
  return new Intl.DateTimeFormat('en-US', { month: 'short', day: 'numeric', year: 'numeric', hour: 'numeric', minute: '2-digit' }).format(new Date(value))
}

function initials(name) {
  return name?.split(' ').slice(0, 2).map((part) => part[0]).join('').toUpperCase() || '?'
}

export default function RegistrationTable({ items, loading }) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full min-w-[1120px] border-collapse text-left">
        <thead>
          <tr className="border-b border-emerald-100 bg-emerald-50/60 text-[11px] font-bold uppercase tracking-[0.11em] text-emerald-900">
            <th className="px-5 py-4">Registration No.</th>
            <th className="px-5 py-4">Date</th>
            <th className="px-5 py-4">Participant</th>
            <th className="px-5 py-4">Car</th>
            <th className="px-5 py-4">Type</th>
            <th className="px-5 py-4">Status</th>
            <th className="px-5 py-4 text-right">Fee / Paid</th>
            <th className="px-5 py-4">Payment</th>
          </tr>
        </thead>
        <tbody className={`divide-y divide-slate-100 transition-opacity ${loading ? 'opacity-45' : 'opacity-100'}`}>
          {items.map((item) => (
            <tr key={item.id} className="transition hover:bg-emerald-50/40">
              <td className="whitespace-nowrap px-5 py-4"><span className="font-mono text-[13px] font-semibold text-emerald-700">{item.registrationNumber}</span></td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-slate-600">{formatDate(item.registrationDate)}</td>
              <td className="px-5 py-4">
                <div className="flex items-center gap-3">
                  <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-slate-100 text-xs font-bold text-slate-600">{initials(item.participantName)}</div>
                  <div className="min-w-0">
                    <p className="font-bold text-slate-900">{item.participantName}</p>
                    <p className="mt-0.5 text-xs text-slate-500">{item.contactNumber} · {item.email}</p>
                  </div>
                </div>
              </td>
              <td className="px-5 py-4"><p className="font-semibold text-slate-800">{item.carMakeModel}</p><p className="mt-1 font-mono text-xs font-bold text-slate-400">{item.plateNumber}</p></td>
              <td className="px-5 py-4"><span className="rounded-lg bg-slate-100 px-2.5 py-1.5 text-xs font-bold text-slate-600">{item.registrationType}</span></td>
              <td className="px-5 py-4"><StatusBadge status={item.status} /></td>
              <td className="whitespace-nowrap px-5 py-4 text-right text-sm"><span className="font-bold text-slate-800">{peso.format(item.registrationFee)}</span><span className="mx-1 text-slate-300">/</span><span className="font-medium text-slate-500">{peso.format(item.amountPaid)}</span></td>
              <td className="px-5 py-4"><PaymentBadge paymentStatus={item.paymentStatus} /></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
