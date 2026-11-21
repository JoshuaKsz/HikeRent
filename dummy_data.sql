
-- ==========================================
-- 1. USERS
-- UserId 1 diasumsikan sudah ada (Admin)
-- ==========================================

INSERT INTO public."Users"
VALUES
(2, 'Budi Santoso', 'budi@geartrack.com', '$2b$12$dummyBudiHash', 'Staff', '081234567802', TRUE, NOW()),
(3, 'Siti Aminah', 'siti@geartrack.com', '$2b$12$dummySitiHash', 'Staff', '081234567803', TRUE, NOW()),
(4, 'Andi Pratama', 'andi@geartrack.com', '$2b$12$dummyAndiHash', 'Staff', '081234567804', TRUE, NOW()),
(5, 'Rina Putri', 'rina@geartrack.com', '$2b$12$dummyRinaHash', 'Staff', '081234567805', TRUE, NOW())
ON CONFLICT DO NOTHING;


-- ==========================================
-- 2. CATEGORIES
-- ==========================================

INSERT INTO public."Categories"
VALUES
(1, 'Camera', 'Kamera untuk fotografi dan videografi'),
(2, 'Lens', 'Lensa kamera berbagai jenis'),
(3, 'Lighting', 'Peralatan pencahayaan fotografi'),
(4, 'Audio', 'Peralatan perekaman audio'),
(5, 'Tripod', 'Tripod dan stabilizer kamera'),
(6, 'Accessories', 'Aksesoris pendukung fotografi')
ON CONFLICT DO NOTHING;


-- ==========================================
-- 3. BRANDS
-- ==========================================

INSERT INTO public."Brands"
VALUES
(1, 'Canon', 'Brand kamera dan lensa'),
(2, 'Sony', 'Brand kamera mirrorless dan audio'),
(3, 'Nikon', 'Brand kamera dan lensa'),
(4, 'Godox', 'Brand lighting fotografi'),
(5, 'DJI', 'Brand drone dan stabilizer')
ON CONFLICT DO NOTHING;


-- ==========================================
-- 4. GEAR ITEMS
-- ==========================================

INSERT INTO public."GearItems"
VALUES
(1, 1, 1, 'Canon EOS R6', 'Full frame mirrorless camera', 350000, 35000000, 10, 1, 0, NULL, TRUE),
(2, 1, 2, 'Sony A7 III', 'Full frame mirrorless camera', 300000, 28000000, 8, 1, 1, NULL, TRUE),
(3, 1, 3, 'Nikon D750', 'Full frame DSLR camera', 250000, 20000000, 6, 0, 0, NULL, TRUE),
(4, 2, 1, 'Canon RF 24-70mm', 'Standard zoom lens', 200000, 18000000, 8, 1, 0, NULL, TRUE),
(5, 2, 2, 'Sony FE 50mm F1.8', 'Prime lens', 100000, 3500000, 12, 0, 0, NULL, TRUE),
(6, 2, 3, 'Nikon 70-200mm', 'Telephoto zoom lens', 250000, 25000000, 5, 1, 0, NULL, TRUE),
(7, 3, 4, 'Godox SL60W', 'LED video light', 100000, 2500000, 10, 1, 0, NULL, TRUE),
(8, 3, 4, 'Godox AD200 Pro', 'Portable flash light', 125000, 4500000, 8, 0, 0, NULL, TRUE),
(9, 3, 4, 'Godox Softbox', 'Softbox lighting modifier', 50000, 800000, 15, 0, 1, NULL, TRUE),
(10, 4, 2, 'Sony Wireless Mic', 'Wireless microphone system', 150000, 5000000, 7, 1, 0, NULL, TRUE),
(11, 4, 2, 'Sony Shotgun Mic', 'Shotgun microphone', 100000, 3000000, 6, 0, 0, NULL, TRUE),
(12, 5, 5, 'DJI RS 3', 'Camera gimbal stabilizer', 200000, 7500000, 5, 1, 0, NULL, TRUE),
(13, 5, 5, 'DJI RS 4', 'Professional camera stabilizer', 250000, 9000000, 4, 0, 0, NULL, TRUE),
(14, 6, 1, 'Canon Battery LP-E6', 'Camera spare battery', 30000, 900000, 20, 0, 0, NULL, TRUE),
(15, 6, 2, 'Sony SD Card 128GB', 'High speed memory card', 25000, 600000, 20, 0, 0, NULL, TRUE)
ON CONFLICT DO NOTHING;


-- ==========================================
-- 5. CUSTOMERS
-- IdentityNumber = 16 digit
-- ==========================================

INSERT INTO public."Customers"
VALUES
(1, 'Ahmad Fauzi', 'ahmad@gmail.com', '081290000001', 'Jakarta Selatan', '3174011201900001', NOW()),
(2, 'Dewi Lestari', 'dewi@gmail.com', '081290000002', 'Jakarta Barat', '3174022302920002', NOW()),
(3, 'Rizky Ramadhan', 'rizky@gmail.com', '081290000003', 'Jakarta Timur', '3174031501950003', NOW()),
(4, 'Putri Anggraini', 'putri@gmail.com', '081290000004', 'Depok, Jawa Barat', '3276014508960004', NOW()),
(5, 'Fajar Nugraha', 'fajar@gmail.com', '081290000005', 'Bekasi, Jawa Barat', '3275010801990005', NOW()),
(6, 'Nadia Safitri', 'nadia@gmail.com', '081290000006', 'Tangerang, Banten', '3671012703970006', NOW()),
(7, 'Dimas Saputra', 'dimas@gmail.com', '081290000007', 'Bogor, Jawa Barat', '3201011801980007', NOW()),
(8, 'Maya Sari', 'maya@gmail.com', '081290000008', 'Jakarta Utara', '3175012209940008', NOW()),
(9, 'Arif Setiawan', 'arif@gmail.com', '081290000009', 'Jakarta Pusat', '3171021101930009', NOW()),
(10, 'Citra Permata', 'citra@gmail.com', '081290000010', 'Bandung, Jawa Barat', '3273013008960010', NOW())
ON CONFLICT DO NOTHING;


-- ==========================================
-- 6. RENTALS
--
-- RentalStatus:
-- Ongoing  = 1
-- Returned = 2
-- Completed = 3
--
-- PaymentStatus:
-- Unpaid = 1
-- Paid   = 2
-- ==========================================

INSERT INTO public."Rentals"
VALUES
(1, 1, 2, 'RNT-2026-001', NOW() - INTERVAL '20 days', CURRENT_DATE - 19, CURRENT_DATE - 16, 3, 1200000, 0, 0, 0, 1200000, 2, NOW()),
(2, 2, 3, 'RNT-2026-002', NOW() - INTERVAL '15 days', CURRENT_DATE - 14, CURRENT_DATE - 11, 3, 900000, 100000, 0, 0, 1000000, 2, NOW()),
(3, 3, 2, 'RNT-2026-003', NOW() - INTERVAL '10 days', CURRENT_DATE - 9, CURRENT_DATE - 6, 3, 1050000, 0, 200000, 0, 1250000, 2, NOW()),
(4, 4, 4, 'RNT-2026-004', NOW() - INTERVAL '5 days', CURRENT_DATE - 4, CURRENT_DATE - 1, 3, 600000, 0, 0, 300000, 900000, 2, NOW()),
(5, 5, 3, 'RNT-2026-005', NOW() - INTERVAL '2 days', CURRENT_DATE - 1, CURRENT_DATE + 2, 1, 1050000, 0, 0, 0, 1050000, 2, NOW()),
(6, 6, 4, 'RNT-2026-006', NOW() - INTERVAL '1 day', CURRENT_DATE, CURRENT_DATE + 3, 1, 1200000, 0, 0, 0, 1200000, 2, NOW()),
(7, 7, 2, 'RNT-2026-007', NOW(), CURRENT_DATE + 1, CURRENT_DATE + 4, 3, 750000, 0, 0, 0, 750000, 1, NOW()),
(8, 8, 5, 'RNT-2026-008', NOW(), CURRENT_DATE + 2, CURRENT_DATE + 5, 3, 600000, 0, 0, 0, 600000, 1, NOW()),
(9, 9, 3, 'RNT-2026-009', NOW() - INTERVAL '3 days', CURRENT_DATE - 2, CURRENT_DATE + 1, 1, 1500000, 0, 0, 0, 1500000, 2, NOW()),
(10, 10, 4, 'RNT-2026-010', NOW(), CURRENT_DATE + 3, CURRENT_DATE + 6, 1, 0, 0, 0, 0, 0, 1, NOW())
ON CONFLICT DO NOTHING;


-- ==========================================
-- 7. RENTAL DETAILS
-- ==========================================

INSERT INTO public."RentalDetails"
VALUES
(1, 1, 1, 1, 350000, 35000000, 3, 1050000),
(2, 1, 14, 1, 30000, 900000, 5, 150000),
(3, 2, 2, 1, 300000, 28000000, 3, 900000),
(4, 3, 4, 1, 200000, 18000000, 3, 600000),
(5, 3, 5, 1, 100000, 3500000, 3, 300000),
(6, 3, 15, 2, 25000, 600000, 3, 150000),
(7, 4, 7, 2, 100000, 2500000, 3, 600000),
(8, 5, 1, 1, 350000, 35000000, 3, 1050000),
(9, 6, 12, 1, 200000, 7500000, 3, 600000),
(10, 6, 10, 1, 150000, 5000000, 4, 600000),
(11, 7, 3, 1, 250000, 20000000, 3, 750000),
(12, 8, 8, 2, 125000, 4500000, 3, 750000),
(13, 9, 1, 1, 350000, 35000000, 3, 1050000),
(14, 9, 6, 1, 250000, 25000000, 3, 750000),
(15, 10, 13, 1, 250000, 9000000, 3, 750000)
ON CONFLICT DO NOTHING;


-- ==========================================
-- 8. PAYMENTS
--
-- PaymentMethod:
-- Cash     = 1
-- QRIS     = 2
-- Transfer = 3
-- Debit    = 4
-- ==========================================

INSERT INTO public."Payments"
VALUES
(1, 1, 2, 'PAY-2026-001', 1200000, 3, NOW() - INTERVAL '16 days'),
(2, 2, 3, 'PAY-2026-002', 1000000, 1, NOW() - INTERVAL '11 days'),
(3, 3, 2, 'PAY-2026-003', 1250000, 2, NOW() - INTERVAL '6 days'),
(4, 4, 4, 'PAY-2026-004', 900000, 3, NOW() - INTERVAL '1 day'),
(5, 5, 3, 'PAY-2026-005', 1050000, 2, NOW()),
(6, 6, 4, 'PAY-2026-006', 1200000, 3, NOW()),
(7, 9, 3, 'PAY-2026-007', 1500000, 1, NOW())
ON CONFLICT DO NOTHING;


-- ==========================================
-- 9. RETURN RECORDS
-- ==========================================

INSERT INTO public."ReturnRecords"
VALUES
(1, 1, 2, 1, 0, 0, 'Barang kembali dalam kondisi baik', 0, 0, 0, NOW() - INTERVAL '16 days'),
(2, 2, 2, 1, 0, 0, 'Baterai kembali normal', 0, 0, 0, NOW() - INTERVAL '16 days'),
(3, 3, 3, 1, 0, 0, 'Kamera terlambat dikembalikan', 100000, 0, 0, NOW() - INTERVAL '10 days'),
(4, 4, 2, 1, 0, 0, 'Lensa kondisi baik', 0, 0, 0, NOW() - INTERVAL '6 days'),
(5, 5, 2, 1, 1, 0, 'Lensa mengalami kerusakan', 0, 200000, 0, NOW() - INTERVAL '6 days'),
(6, 6, 2, 2, 0, 0, 'Memory card lengkap', 0, 0, 0, NOW() - INTERVAL '6 days'),
(7, 7, 4, 2, 0, 2, 'Dua unit lampu hilang', 0, 0, 300000, NOW() - INTERVAL '1 day')
ON CONFLICT DO NOTHING;


-- ==========================================
-- 10. ACTIVITY LOGS
-- ==========================================

INSERT INTO public."ActivityLogs"
VALUES
(1, 1, 'Admin login ke sistem', NOW() - INTERVAL '20 days'),
(2, 2, 'Membuat rental RNT-2026-001', NOW() - INTERVAL '20 days'),
(3, 3, 'Membuat rental RNT-2026-002', NOW() - INTERVAL '15 days'),
(4, 2, 'Memproses pengembalian RNT-2026-001', NOW() - INTERVAL '16 days'),
(5, 4, 'Mencatat pembayaran RNT-2026-004', NOW() - INTERVAL '5 days'),
(6, 3, 'Memproses rental RNT-2026-005', NOW() - INTERVAL '2 days'),
(7, 4, 'Memproses pengembalian RNT-2026-004', NOW() - INTERVAL '1 day'),
(8, 5, 'Membuat rental RNT-2026-008', NOW()),
(9, 3, 'Mencatat pembayaran RNT-2026-009', NOW()),
(10, 1, 'Melihat laporan transaksi', NOW())
ON CONFLICT DO NOTHING;


-- ==========================================
-- 11. RESET SEQUENCES
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
