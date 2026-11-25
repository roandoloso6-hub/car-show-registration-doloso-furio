import KpiCard from './KpiCard.jsx'

const peso = new Intl.NumberFormat('en-PH', { style: 'currency', currency: 'PHP', minimumFractionDigits: 2 })

const Icons = {
  registrations: <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" className="h-5 w-5"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75"/></svg>,
  money: <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" className="h-5 w-5"><rect x="2" y="5" width="20" height="14" rx="2"/><path d="M16 12h.01M2 9h20"/></svg>,
  uncollected: <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" className="h-5 w-5"><circle cx="12" cy="12" r="9"/><path d="M12 7v5l3 2"/></svg>,
  confirmed: <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" className="h-5 w-5"><circle cx="12" cy="12" r="9"/><path d="m8 12 2.5 2.5L16 9"/></svg>,
  checked: <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" className="h-5 w-5"><path d="M20 10c0 5-3.5 9-8 11-4.5-2-8-6-8-11V5l8-3 8 3v5Z"/><path d="m8.5 12 2.2 2.2 4.8-5"/></svg>,
}

export default function KpiCards({ summary }) {
  const value = (key) => summary?.[key] ?? 0
  return (
    <section className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-5">
      <KpiCard label="Total Registrations" value={value('totalRegistrations')} tone="emerald" icon={Icons.registrations} />
      <KpiCard label="Total Payments" value={peso.format(value('totalPayments'))} tone="emerald" icon={Icons.money} />
      <KpiCard label="Uncollected Payments" value={peso.format(value('uncollectedPayments'))} tone="amber" icon={Icons.uncollected} />
      <KpiCard label="Confirmed" value={value('confirmed')} tone="emerald" icon={Icons.confirmed} />
      <KpiCard label="Checked In" value={value('checkedIn')} tone="emerald" icon={Icons.checked} />
    </section>
  )
}
