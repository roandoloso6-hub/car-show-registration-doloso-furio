export default function KpiCard({ label, value, tone = 'emerald', icon }) {
  const tones = {
    emerald: 'bg-emerald-50 text-emerald-700 ring-emerald-100',
    forest: 'bg-emerald-100/70 text-emerald-800 ring-emerald-200',
    amber: 'bg-amber-50 text-amber-600 ring-amber-100',
  }

  return (
    <article className="group relative overflow-hidden rounded-2xl border border-emerald-100 bg-white p-5 shadow-[0_1px_2px_rgba(15,23,42,0.03),0_8px_24px_rgba(15,23,42,0.04)] transition duration-200 hover:-translate-y-0.5 hover:border-emerald-300 hover:shadow-[0_12px_30px_rgba(6,95,70,0.08)]">
      <div className="absolute inset-x-0 top-0 h-0.5 bg-gradient-to-r from-transparent via-emerald-300 to-transparent opacity-0 transition group-hover:opacity-100" />
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0">
          <p className="text-[12px] font-bold uppercase tracking-[0.08em] text-slate-500">{label}</p>
          <p className="mt-3 text-[26px] font-bold leading-none tracking-tight text-slate-950">{value}</p>
        </div>
        {icon && (
          <div className={`flex h-11 w-11 shrink-0 items-center justify-center rounded-xl ring-1 ${tones[tone]}`}>
            {icon}
          </div>
        )}
      </div>
    </article>
  )
}
