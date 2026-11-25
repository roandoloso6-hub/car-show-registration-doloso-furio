const types = ['Exhibitor', 'Competitor', 'VIP']
const statuses = ['Pending', 'Confirmed', 'CheckedIn', 'Cancelled']
const names = ['Juan Dela Cruz','Maria Santos','Jose Ramirez','Angelica Reyes','Mark Anthony Cruz','Jessa Mae Bonifacio','Ramil Ocampo','Kathleen Navarro','Carlo Mendoza','Lyka Garcia','Paolo Villanueva','Mae Christine Flores','Alfredo Perez','Christine Joy Morales','Ernesto Dominguez','Lovely Ann Ramos','Miguel Antonio Garcia','Kristine Mae Bautista','Danilo Mercado','Patricia Isabel Lim','Arvin Castillo','Rowena Pascual','Gabriel Lorenzo','Janelle Camille Torres','Francis David Aquino','Maricel Manalo','Neil Patrick Santos','Hazel Marie Gonzales','Ronald Velasco','Andrea Nicole Rivera']
const cars = ['Toyota Supra MK4','Honda Civic Type R','Toyota GR Yaris','Mazda MX-5 Miata','Mitsubishi Lancer Evolution X','Nissan GT-R R35','Ford Mustang GT','Honda Accord 2.0','Toyota Fortuner GR Sport','Nissan Navara Calibre','Honda Civic VTi','Toyota Corolla Altis','Mitsubishi Montero Sport','Subaru WRX STI','Nissan Patrol','Toyota Hilux GR Sport','Mazda3 SkyActiv','Honda CR-V','Toyota Camry','Hyundai Tucson','Kia Carnival','Mitsubishi Xpander Cross','Ford Ranger Raptor','Isuzu mu-X','BMW 320i','Mercedes-Benz C180','Toyota Land Cruiser Prado','Suzuki Jimny','Chevrolet Camaro SS','Porsche 911 Carrera']

export const sampleRegistrations = names.map((participantName, index) => {
  const i = index + 1
  const registrationType = types[index % types.length]
  const status = statuses[index % statuses.length]
  const fee = registrationType === 'Exhibitor' ? 1500 : registrationType === 'Competitor' ? 2500 : 5000
  const amountPaid = status === 'Cancelled' ? 0 : index % 3 === 0 ? 0 : index % 3 === 1 ? fee / 2 : fee

  return {
    id: `sample-${i.toString().padStart(4, '0')}`,
    registrationNumber: `CS27-${i.toString().padStart(4, '0')}`,
    registrationDate: `2027-02-${String((i % 28) + 1).padStart(2, '0')}T${String(8 + (i % 9)).padStart(2, '0')}:${String((i * 7) % 60).padStart(2, '0')}:00`,
    participantName,
    contactNumber: `0917${String(1234500 + i).padStart(7, '0')}`,
    email: `${participantName.toLower().replaceAll(' ', '')}${i}@gmail.com`,
    address: `${100 + i} Rizal Street, Legazpi City, Albay`,
    carMakeModel: cars[index],
    plateNumber: `CS27${i.toString().padStart(3, '0')}`,
    registrationType,
    status,
    registrationFee: fee,
    amountPaid,
    paymentStatus: amountPaid === 0 ? 'Unpaid' : amountPaid < fee ? 'Partial' : 'Paid',
  }
})

export async function getRegistrations({ participantName = '', type = '', status = '', page = 1, pageSize = 10 } = {}) {
  let items = [...sampleRegistrations]
  const needle = participantName.trim().toLower()
  if (needle) items = items.filter((item) => item.participantName.toLower().includes(needle))
  if (type) items = items.filter((item) => item.registrationType === type)
  if (status) items = items.filter((item) => item.status === status)

  const totalCount = items.length
  const totalPages = Math.ceil(totalCount / pageSize)
  const safePage = Math.min(Math.max(page, 1), Math.max(totalPages, 1))
  const start = (safePage - 1) * pageSize

  return { items: items.slice(start, start + pageSize), page: safePage, pageSize, totalCount, totalPages }
}

export async function getSummary() {
  const active = sampleRegistrations.filter((item) => item.status !== 'Cancelled')
  return {
    totalRegistrations: sampleRegistrations.length,
    totalFees: active.reduce((sum, item) => sum + item.registrationFee, 0),
    totalPayments: active.reduce((sum, item) => sum + item.amountPaid, 0),
    uncollectedPayments: active.reduce((sum, item) => sum + item.registrationFee - item.amountPaid, 0),
    confirmed: sampleRegistrations.filter((item) => item.status === 'Confirmed').length,
    checkedIn: sampleRegistrations.filter((item) => item.status === 'CheckedIn').length,
    pending: sampleRegistrations.filter((item) => item.status === 'Pending').length,
    cancelled: sampleRegistrations.filter((item) => item.status === 'Cancelled').length,
  }
}
