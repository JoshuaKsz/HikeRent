-- ==========================================
-- DUMMY DATA FOR HIKERENT SYSTEM
-- (Peralatan Pendakian Gunung & Outdoor)
-- ==========================================

-- Catatan: Tabel Users tidak diisi di sini karena
-- staff dibuat oleh StaffSeeder.cs saat aplikasi start.
-- UserId = 1 diasumsikan sebagai staff/admin aktif.


-- ==========================================
-- 1. CATEGORIES
-- ==========================================

INSERT INTO public."Categories" ("CategoryId", "CategoryName", "Description")
VALUES
(1, 'Tenda', 'Tenda kemping berbagai kapasitas dan spesifikasi pendakian'),
(2, 'Tas Carrier', 'Backpack dan ransel pendakian kapasitas 40L hingga 80L'),
(3, 'Sleeping Bag & Matras', 'Peralatan tidur, kantong tidur, dan matras isolasi dingin'),
(4, 'Alat Masak & Logistik', 'Kompor outdoor, cooking set (nesting), dan peralatan makan'),
(5, 'Penerangan & Navigasi', 'Headlamp LED, senter outdoor, dan alat navigasi lapangan'),
(6, 'Sepatu & Trekking Pole', 'Sepatu gunung, gaiter, dan tongkat pendakian (trekking pole)')
ON CONFLICT ("CategoryId") DO NOTHING;


-- ==========================================
-- 2. BRANDS
-- ==========================================

INSERT INTO public."Brands" ("BrandId", "BrandName", "Description")
VALUES
(1, 'Eiger', 'Brand perlengkapan outdoor dan mountaineering asal Indonesia'),
(2, 'Consina', 'Brand peralatan kegiatan luar ruangan dan adventure lokal'),
(3, 'Rei Outdoor', 'Brand peralatan camping dan pendakian populer'),
(4, 'Naturehike', 'Brand spesialis peralatan camping ultralight internasional'),
(5, 'Deuter', 'Brand ransel dan carrier mountaineering premium dari Jerman')
ON CONFLICT ("BrandId") DO NOTHING;


-- ==========================================
-- 3. GEAR ITEMS
-- ==========================================

INSERT INTO public."GearItems" (
    "GearItemId", "CategoryId", "BrandId", "GearName", "Description",
    "DailyPrice", "ReplacementPrice", "TotalStock", "DamagedStock", "ImageUrl", "IsActive"
)
VALUES
(1, 1, 4, 'Tenda Naturehike Cloud Up 2', 'Tenda ultralight kapasitas 2 orang, waterproof 3000mm, silnylon', 45000.00, 1200000.00, 8, 0, 'assets/tenda.png', TRUE),
(2, 1, 2, 'Tenda Consina Magnum 4', 'Tenda dome kapasitas 4 orang, double layer dengan vestibule luas', 60000.00, 1500000.00, 10, 1, 'assets/tenda.png', TRUE),
(3, 1, 1, 'Tenda Eiger Shield 4P', 'Tenda gunung 4 musim frame alumunium, tahan angin kencang', 85000.00, 2800000.00, 5, 0, 'assets/tenda.png', TRUE),
(4, 2, 5, 'Carrier Deuter Futura Pro 45L', 'Tas gunung ergonomis sistem sirkulasi udara Aircomfort', 75000.00, 2400000.00, 6, 0, 'assets/carrier.png', TRUE),
(5, 2, 1, 'Carrier Eiger Rhinos 60L', 'Tas carrier kapasitas besar dengan raincover terintegrasi', 80000.00, 2200000.00, 8, 1, 'assets/carrier.png', TRUE),
(6, 2, 2, 'Carrier Consina Centipede 50L', 'Tas carrier medium cocok untuk pendakian 2-3 hari', 50000.00, 1100000.00, 10, 0, 'assets/carrier.png', TRUE),
(7, 3, 4, 'Sleeping Bag Naturehike LW180', 'Sleeping bag dacron hangat, ultra ringkas dan ringan', 25000.00, 450000.00, 15, 0, 'assets/sleeping_bag.png', TRUE),
(8, 3, 3, 'Sleeping Bag Rei Summit', 'Sleeping bag bulu sintetis tahan suhu ekstrem hingga 5 derajat C', 40000.00, 850000.00, 10, 0, 'assets/sleeping_bag.png', TRUE),
(9, 3, 4, 'Matras Tiup Naturehike FC-10', 'Matras tiup kenyamanan ekstra isolasi dingin tanah', 20000.00, 350000.00, 12, 0, NULL, TRUE),
(10, 4, 4, 'Kompor Portable Naturehike Spider', 'Kompor gas selang lipat anti angin high heat output', 30000.00, 400000.00, 12, 0, NULL, TRUE),
(11, 4, 3, 'Cooking Set Nesting 2', 'Panci alumunium anodized ringan untuk 2-3 orang', 25000.00, 350000.00, 10, 0, NULL, TRUE),
(12, 5, 1, 'Headlamp Eiger Vektor 300L', 'Senter kepala LED 300 lumens waterproof mode merah & putih', 20000.00, 300000.00, 15, 0, NULL, TRUE),
(13, 6, 2, 'Trekking Pole Consina Antishock', 'Tongkat pendakian duralumin dengan fitur antishock', 25000.00, 300000.00, 14, 0, NULL, TRUE)
ON CONFLICT ("GearItemId") DO NOTHING;


-- ==========================================
-- 4. CUSTOMERS
-- ==========================================

INSERT INTO public."Customers" ("CustomerId", "FullName", "Email", "PhoneNumber", "Address", "IdentityNumber", "CreatedAt")
VALUES
(1, 'Ahmad Fauzi', 'ahmad.fauzi@gmail.com', '081290000001', 'Jl. Tebet Raya No. 12, Jakarta Selatan', '3174011201900001', NOW() - INTERVAL '30 days'),
(2, 'Dewi Lestari', 'dewi.lestari@gmail.com', '081290000002', 'Jl. Kebon Jeruk No. 45, Jakarta Barat', '3174022302920002', NOW() - INTERVAL '25 days'),
(3, 'Rizky Ramadhan', 'rizky.ramadhan@gmail.com', '081290000003', 'Jl. Duren Sawit No. 88, Jakarta Timur', '3174031501950003', NOW() - INTERVAL '20 days'),
(4, 'Putri Anggraini', 'putri.anggraini@gmail.com', '081290000004', 'Jl. Margonda Raya No. 102, Depok', '3276014508960004', NOW() - INTERVAL '15 days'),
(5, 'Fajar Nugraha', 'fajar.nugraha@gmail.com', '081290000005', 'Jl. Ahmad Yani No. 15, Bekasi', '3275010801990005', NOW() - INTERVAL '12 days'),
(6, 'Nadia Safitri', 'nadia.safitri@gmail.com', '081290000006', 'Jl. BSD Grand Boulevard, Tangerang', '3671012703970006', NOW() - INTERVAL '10 days'),
(7, 'Dimas Saputra', 'dimas.saputra@gmail.com', '081290000007', 'Jl. Pajajaran No. 7, Bogor', '3201011801980007', NOW() - INTERVAL '7 days'),
(8, 'Maya Sari', 'maya.sari@gmail.com', '081290000008', 'Jl. Sunter Permai No. 3, Jakarta Utara', '3175012209940008', NOW() - INTERVAL '5 days'),
(9, 'Arif Setiawan', 'arif.setiawan@gmail.com', '081290000009', 'Jl. Cempaka Putih No. 21, Jakarta Pusat', '3171021101930009', NOW() - INTERVAL '3 days'),
(10, 'Citra Permata', 'citra.permata@gmail.com', '081290000010', 'Jl. Dago No. 54, Bandung', '3273013008960010', NOW() - INTERVAL '1 day')
ON CONFLICT ("CustomerId") DO NOTHING;


-- ==========================================
-- 5. RENTALS
-- Status: Ongoing | Returned | Completed
-- PaymentStatus: Unpaid | Paid
-- ==========================================

INSERT INTO public."Rentals" (
    "RentalId", "CustomerId", "UserId", "RentalCode", "RentalDate",
    "PlannedStartDate", "PlannedReturnDate", "Status",
    "TotalRentalFee", "TotalLateFee", "TotalDamageFee", "TotalLostItemFee", "GrandTotal",
    "PaymentStatus", "CreatedAt"
)
VALUES
(1, 1, 1, 'RNT-20261101-001', NOW() - INTERVAL '20 days', CURRENT_DATE - 20, CURRENT_DATE - 17, 'Completed', 450000.00, 0.00, 0.00, 0.00, 450000.00, 'Paid', NOW() - INTERVAL '20 days'),
(2, 2, 1, 'RNT-20261105-002', NOW() - INTERVAL '15 days', CURRENT_DATE - 15, CURRENT_DATE - 12, 'Completed', 420000.00, 42000.00, 0.00, 0.00, 462000.00, 'Paid', NOW() - INTERVAL '15 days'),
(3, 3, 1, 'RNT-20261110-003', NOW() - INTERVAL '10 days', CURRENT_DATE - 10, CURRENT_DATE - 7, 'Completed', 480000.00, 0.00, 300000.00, 0.00, 780000.00, 'Paid', NOW() - INTERVAL '10 days'),
(4, 4, 1, 'RNT-20261115-004', NOW() - INTERVAL '5 days', CURRENT_DATE - 5, CURRENT_DATE - 2, 'Completed', 360000.00, 0.00, 0.00, 300000.00, 660000.00, 'Paid', NOW() - INTERVAL '5 days'),
(5, 5, 1, 'RNT-20261118-005', NOW() - INTERVAL '3 days', CURRENT_DATE - 2, CURRENT_DATE + 1, 'Ongoing', 480000.00, 0.00, 0.00, 0.00, 480000.00, 'Paid', NOW() - INTERVAL '3 days'),
(6, 6, 1, 'RNT-20261120-006', NOW() - INTERVAL '2 days', CURRENT_DATE - 1, CURRENT_DATE + 2, 'Ongoing', 495000.00, 0.00, 0.00, 0.00, 495000.00, 'Paid', NOW() - INTERVAL '2 days'),
(7, 7, 1, 'RNT-20261121-007', NOW() - INTERVAL '1 day', CURRENT_DATE, CURRENT_DATE + 3, 'Ongoing', 375000.00, 0.00, 0.00, 0.00, 375000.00, 'Unpaid', NOW() - INTERVAL '1 day'),
(8, 8, 1, 'RNT-20261122-008', NOW(), CURRENT_DATE + 1, CURRENT_DATE + 4, 'Ongoing', 240000.00, 0.00, 0.00, 0.00, 240000.00, 'Unpaid', NOW())
ON CONFLICT ("RentalId") DO NOTHING;


-- ==========================================
-- 6. RENTAL DETAILS
-- Subtotal = DailyPrice x Quantity x RentalDays
-- ==========================================

INSERT INTO public."RentalDetails" (
    "RentalDetailId", "RentalId", "GearItemId", "Quantity",
    "DailyPrice", "ReplacementPrice", "RentalDays", "Subtotal"
)
VALUES
-- Rental 1: 135k + 150k + 90k + 75k = 450k
(1, 1, 1, 1, 45000.00, 1200000.00, 3, 135000.00),
(2, 1, 7, 2, 25000.00, 450000.00, 3, 150000.00),
(3, 1, 10, 1, 30000.00, 400000.00, 3, 90000.00),
(4, 1, 11, 1, 25000.00, 350000.00, 3, 75000.00),

-- Rental 2: 180k + 240k = 420k
(5, 2, 2, 1, 60000.00, 1500000.00, 3, 180000.00),
(6, 2, 8, 2, 40000.00, 850000.00, 3, 240000.00),

-- Rental 3: 255k + 225k = 480k
(7, 3, 3, 1, 85000.00, 2800000.00, 3, 255000.00),
(8, 3, 4, 1, 75000.00, 2400000.00, 3, 225000.00),

-- Rental 4: 240k + 120k = 360k
(9, 4, 5, 1, 80000.00, 2200000.00, 3, 240000.00),
(10, 4, 12, 2, 20000.00, 300000.00, 3, 120000.00),

-- Rental 5: 180k + 150k + 150k = 480k
(11, 5, 2, 1, 60000.00, 1500000.00, 3, 180000.00),
(12, 5, 6, 1, 50000.00, 1100000.00, 3, 150000.00),
(13, 5, 7, 2, 25000.00, 450000.00, 3, 150000.00),

-- Rental 6: 225k + 120k + 150k = 495k
(14, 6, 4, 1, 75000.00, 2400000.00, 3, 225000.00),
(15, 6, 8, 1, 40000.00, 850000.00, 3, 120000.00),
(16, 6, 13, 2, 25000.00, 300000.00, 3, 150000.00),

-- Rental 7: 255k + 120k = 375k
(17, 7, 3, 1, 85000.00, 2800000.00, 3, 255000.00),
(18, 7, 9, 2, 20000.00, 350000.00, 3, 120000.00),

-- Rental 8: 150k + 90k = 240k
(19, 8, 6, 1, 50000.00, 1100000.00, 3, 150000.00),
(20, 8, 10, 1, 30000.00, 400000.00, 3, 90000.00)
ON CONFLICT ("RentalDetailId") DO NOTHING;


-- ==========================================
-- 7. PAYMENTS
-- PaymentMethod: Cash | QRIS | Transfer | Debit
-- ==========================================

INSERT INTO public."Payments" (
    "PaymentId", "RentalId", "UserId", "PaymentCode",
    "Amount", "PaymentMethod", "PaymentDate"
)
VALUES
(1, 1, 1, 'PAY-20261101-001', 450000.00, 'QRIS', NOW() - INTERVAL '20 days'),
(2, 2, 1, 'PAY-20261105-002', 462000.00, 'Cash', NOW() - INTERVAL '12 days'),
(3, 3, 1, 'PAY-20261110-003', 780000.00, 'Transfer', NOW() - INTERVAL '7 days'),
(4, 4, 1, 'PAY-20261115-004', 660000.00, 'Debit', NOW() - INTERVAL '2 days'),
(5, 5, 1, 'PAY-20261118-005', 480000.00, 'QRIS', NOW() - INTERVAL '3 days'),
(6, 6, 1, 'PAY-20261120-006', 495000.00, 'Transfer', NOW() - INTERVAL '2 days')
ON CONFLICT ("PaymentId") DO NOTHING;


-- ==========================================
-- 8. RETURN RECORDS
-- ==========================================

INSERT INTO public."ReturnRecords" (
    "ReturnRecordId", "RentalDetailId", "UserId",
    "QuantityReturned", "DamagedQuantity", "LostQuantity",
    "Notes", "LateFee", "DamageFee", "LostItemFee", "ActualReturnDate"
)
VALUES
-- Rental 1: semua barang kembali baik
(1, 1, 1, 1, 0, 0, 'Tenda bersih dan lengkap', 0.00, 0.00, 0.00, NOW() - INTERVAL '17 days'),
(2, 2, 1, 2, 0, 0, 'Sleeping bag rapi dalam pouch', 0.00, 0.00, 0.00, NOW() - INTERVAL '17 days'),
(3, 3, 1, 1, 0, 0, 'Kompor berfungsi normal', 0.00, 0.00, 0.00, NOW() - INTERVAL '17 days'),
(4, 4, 1, 1, 0, 0, 'Nesting lengkap tidak ada penyok', 0.00, 0.00, 0.00, NOW() - INTERVAL '17 days'),

-- Rental 2: telat 1 hari, denda 10% dari total sewa harian (420k)
(5, 5, 1, 1, 0, 0, 'Terlambat dikembalikan 1 hari', 42000.00, 0.00, 0.00, NOW() - INTERVAL '11 days'),
(6, 6, 1, 2, 0, 0, 'Terlambat dikembalikan 1 hari', 0.00, 0.00, 0.00, NOW() - INTERVAL '11 days'),

-- Rental 3: carrier Deuter robek di bagian bahu, denda kerusakan
(7, 7, 1, 1, 0, 0, 'Tenda gunung baik', 0.00, 0.00, 0.00, NOW() - INTERVAL '7 days'),
(8, 8, 1, 1, 0, 0, 'Carrier Deuter robek di bagian bahu', 0.00, 300000.00, 0.00, NOW() - INTERVAL '7 days'),

-- Rental 4: 1 headlamp hilang
(9, 9, 1, 1, 0, 0, 'Carrier Rhinos baik', 0.00, 0.00, 0.00, NOW() - INTERVAL '2 days'),
(10, 10, 1, 1, 0, 1, 'Satu headlamp hilang di gunung', 0.00, 0.00, 300000.00, NOW() - INTERVAL '2 days')
ON CONFLICT ("ReturnRecordId") DO NOTHING;


-- ==========================================
-- 9. ACTIVITY LOGS
-- ==========================================

INSERT INTO public."ActivityLogs" ("ActivityLogId", "UserId", "Activity", "ActivityDate")
VALUES
(1, 1, 'Staff login ke sistem HikeRent', NOW() - INTERVAL '20 days'),
(2, 1, 'Membuat rental RNT-20261101-001 untuk customer Ahmad Fauzi', NOW() - INTERVAL '20 days'),
(3, 1, 'Menerima pembayaran PAY-20261101-001 via QRIS', NOW() - INTERVAL '20 days'),
(4, 1, 'Memproses pengembalian rental RNT-20261101-001', NOW() - INTERVAL '17 days'),
(5, 1, 'Membuat rental RNT-20261105-002 untuk customer Dewi Lestari', NOW() - INTERVAL '15 days'),
(6, 1, 'Memproses pengembalian RNT-20261105-002 (denda terlambat Rp 42.000)', NOW() - INTERVAL '11 days'),
(7, 1, 'Membuat rental RNT-20261110-003 untuk customer Rizky Ramadhan', NOW() - INTERVAL '10 days'),
(8, 1, 'Memproses pengembalian RNT-20261110-003 (denda kerusakan Rp 300.000)', NOW() - INTERVAL '7 days'),
(9, 1, 'Membuat rental RNT-20261115-004 untuk customer Putri Anggraini', NOW() - INTERVAL '5 days'),
(10, 1, 'Memproses pengembalian RNT-20261115-004 (denda kehilangan Rp 300.000)', NOW() - INTERVAL '2 days'),
(11, 1, 'Membuat rental RNT-20261118-005 untuk customer Fajar Nugraha', NOW() - INTERVAL '3 days'),
(12, 1, 'Menerima pembayaran PAY-20261118-005 via QRIS', NOW() - INTERVAL '3 days'),
(13, 1, 'Membuat rental RNT-20261120-006 untuk customer Nadia Safitri', NOW() - INTERVAL '2 days'),
(14, 1, 'Menerima pembayaran PAY-20261120-006 via Transfer', NOW() - INTERVAL '2 days'),
(15, 1, 'Membuat rental RNT-20261121-007 untuk customer Dimas Saputra', NOW() - INTERVAL '1 day')
ON CONFLICT ("ActivityLogId") DO NOTHING;


-- ==========================================
-- 10. RESET SEQUENCES
-- ==========================================

SELECT setval(
    pg_get_serial_sequence('public."Users"', 'UserId'),
    GREATEST(COALESCE((SELECT MAX("UserId") FROM public."Users"), 1), 1)
);

SELECT setval(
    pg_get_serial_sequence('public."Categories"', 'CategoryId'),
    GREATEST(COALESCE((SELECT MAX("CategoryId") FROM public."Categories"), 1), 1)
);

SELECT setval(
    pg_get_serial_sequence('public."Brands"', 'BrandId'),
    GREATEST(COALESCE((SELECT MAX("BrandId") FROM public."Brands"), 1), 1)
);

SELECT setval(
    pg_get_serial_sequence('public."GearItems"', 'GearItemId'),
    GREATEST(COALESCE((SELECT MAX("GearItemId") FROM public."GearItems"), 1), 1)
);

SELECT setval(
    pg_get_serial_sequence('public."Customers"', 'CustomerId'),
    GREATEST(COALESCE((SELECT MAX("CustomerId") FROM public."Customers"), 1), 1)
);

SELECT setval(
    pg_get_serial_sequence('public."Rentals"', 'RentalId'),
    GREATEST(COALESCE((SELECT MAX("RentalId") FROM public."Rentals"), 1), 1)
);

SELECT setval(
    pg_get_serial_sequence('public."RentalDetails"', 'RentalDetailId'),
    GREATEST(COALESCE((SELECT MAX("RentalDetailId") FROM public."RentalDetails"), 1), 1)
);

SELECT setval(
    pg_get_serial_sequence('public."Payments"', 'PaymentId'),
    GREATEST(COALESCE((SELECT MAX("PaymentId") FROM public."Payments"), 1), 1)
);

SELECT setval(
    pg_get_serial_sequence('public."ReturnRecords"', 'ReturnRecordId'),
    GREATEST(COALESCE((SELECT MAX("ReturnRecordId") FROM public."ReturnRecords"), 1), 1)
);

SELECT setval(
    pg_get_serial_sequence('public."ActivityLogs"', 'ActivityLogId'),
    GREATEST(COALESCE((SELECT MAX("ActivityLogId") FROM public."ActivityLogs"), 1), 1)
);
