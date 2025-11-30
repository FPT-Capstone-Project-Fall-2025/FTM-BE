-- =============================================
-- Seed Data for Fund Management Module
-- Tables: FTFunds, FTFundHistories, FTFundCampaigns
-- =============================================

-- Note: Replace the UUID values with actual IDs from your database
-- Get FamilyTree ID: SELECT "Id" FROM "FamilyTrees" LIMIT 1;
-- Get User ID: SELECT "Id" FROM "ApplicationUser" LIMIT 1;

-- Variables (Replace these with actual IDs from your database)
-- FamilyTree ID: 26259d4d-4ac5-49f4-bebb-93cda81663ed
-- User ID: Will be selected from ApplicationUser table

-- =============================================
-- 1. INSERT SAMPLE FTFUNDS
-- =============================================

-- Fund 1: Main Family Fund
INSERT INTO "FTFunds" (
    "Id",
    "FTId",
    "CurrentMoney",
    "FundNote",
    "FundName",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    '11111111-1111-1111-1111-111111111111',
    '26259d4d-4ac5-49f4-bebb-93cda81663ed',
    50000000.00,
    'Quỹ chính của gia đình, dùng cho các hoạt động chung',
    'Quỹ Gia Đình Chính',
    NOW(),
    'System Admin',
    NOW(),
    'System Admin',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- Fund 2: Education Fund
INSERT INTO "FTFunds" (
    "Id",
    "FTId",
    "CurrentMoney",
    "FundNote",
    "FundName",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    '22222222-2222-2222-2222-222222222222',
    '26259d4d-4ac5-49f4-bebb-93cda81663ed',
    15000000.00,
    'Quỹ hỗ trợ học tập cho các thành viên trẻ trong gia đình',
    'Quỹ Học Bổng',
    NOW(),
    'System Admin',
    NOW(),
    'System Admin',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- Fund 3: Emergency Fund
INSERT INTO "FTFunds" (
    "Id",
    "FTId",
    "CurrentMoney",
    "FundNote",
    "FundName",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    '33333333-3333-3333-3333-333333333333',
    '26259d4d-4ac5-49f4-bebb-93cda81663ed',
    30000000.00,
    'Quỹ dự phòng cho các trường hợp khẩn cấp',
    'Quỹ Khẩn Cấp',
    NOW(),
    'System Admin',
    NOW(),
    'System Admin',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- =============================================
-- 2. INSERT SAMPLE FTFUNDCAMPAIGNS
-- =============================================

-- Campaign 1: Tet Campaign
INSERT INTO "FTFundCampaigns" (
    "Id",
    "FTFundId",
    "CampaignName",
    "CampaignDescription",
    "OrganizerName",
    "OrganizerContact",
    "StartDate",
    "EndDate",
    "TargetAmount",
    "AmountRaised",
    "Status",
    "MediaAttachments",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    'c1111111-1111-1111-1111-111111111111',
    '11111111-1111-1111-1111-111111111111',
    'Gây Quỹ Tết Nguyên Đán 2025',
    'Chiến dịch gây quỹ để tổ chức Tết Nguyên Đán cho toàn bộ gia đình. Bao gồm chi phí tổ chức tiệc, quà tặng, và các hoạt động văn hóa truyền thống.',
    'Ban Tổ Chức Gia Đình',
    'contact@family.com | 0901234567',
    NOW() - INTERVAL '30 days',
    NOW() + INTERVAL '30 days',
    20000000.00,
    8500000.00,
    12001, -- Active
    '["https://example.com/tet-campaign-1.jpg", "https://example.com/tet-campaign-2.jpg"]',
    NOW(),
    'System Admin',
    NOW() - INTERVAL '30 days',
    'System Admin',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- Campaign 2: Ancestor Worship
INSERT INTO "FTFundCampaigns" (
    "Id",
    "FTFundId",
    "CampaignName",
    "CampaignDescription",
    "OrganizerName",
    "OrganizerContact",
    "StartDate",
    "EndDate",
    "TargetAmount",
    "AmountRaised",
    "Status",
    "MediaAttachments",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    'c2222222-2222-2222-2222-222222222222',
    '11111111-1111-1111-1111-111111111111',
    'Lễ Giỗ Tổ 2025',
    'Gây quỹ tổ chức lễ giỗ tổ tiên hàng năm. Chi phí bao gồm lễ vật, hoa quả, và tổ chức tiệc cho các thành viên tham dự.',
    'Trưởng Họ',
    'truongho@family.com | 0912345678',
    NOW() - INTERVAL '15 days',
    NOW() + INTERVAL '45 days',
    15000000.00,
    15000000.00,
    12003, -- Completed
    '["https://example.com/ancestor-worship.jpg"]',
    NOW(),
    'System Admin',
    NOW() - INTERVAL '15 days',
    'System Admin',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- Campaign 3: Scholarship Fund
INSERT INTO "FTFundCampaigns" (
    "Id",
    "FTFundId",
    "CampaignName",
    "CampaignDescription",
    "OrganizerName",
    "OrganizerContact",
    "StartDate",
    "EndDate",
    "TargetAmount",
    "AmountRaised",
    "Status",
    "MediaAttachments",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    'c3333333-3333-3333-3333-333333333333',
    '22222222-2222-2222-2222-222222222222',
    'Học Bổng Sinh Viên 2025',
    'Chiến dịch gây quỹ học bổng cho các em sinh viên trong gia đình đang theo học đại học. Mỗi em sẽ nhận 5 triệu đồng/năm.',
    'Ban Giáo Dục Gia Đình',
    'education@family.com | 0923456789',
    NOW(),
    NOW() + INTERVAL '60 days',
    25000000.00,
    5000000.00,
    12001, -- Active
    '["https://example.com/scholarship-1.jpg", "https://example.com/scholarship-2.jpg", "https://example.com/scholarship-3.jpg"]',
    NOW(),
    'System Admin',
    NOW(),
    'System Admin',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- =============================================
-- 3. INSERT SAMPLE FTFUNDHISTORIES
-- =============================================

-- History 1: Deposit from member
INSERT INTO "FTFundHistories" (
    "Id",
    "FTFundId",
    "MoneyType",
    "MoneyAmount",
    "FundDescription",
    "FundEvent",
    "Recipient",
    "Status",
    "ApprovedBy",
    "ApprovedOn",
    "ApprovalFeedback",
    "PaymentTransactionId",
    "CampaignId",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    'h1111111-1111-1111-1111-111111111111',
    '11111111-1111-1111-1111-111111111111',
    10001, -- Deposit
    5000000.00,
    'Đóng góp hàng tháng vào quỹ gia đình',
    'Đóng góp định kỳ tháng 11/2025',
    NULL,
    11001, -- Approved
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1),
    NOW(),
    'Đã duyệt và ghi nhận vào quỹ',
    'TXN-2025110401',
    NULL,
    NOW(),
    'System Admin',
    NOW() - INTERVAL '2 days',
    'Nguyễn Văn A',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- History 2: Deposit for Tet campaign
INSERT INTO "FTFundHistories" (
    "Id",
    "FTFundId",
    "MoneyType",
    "MoneyAmount",
    "FundDescription",
    "FundEvent",
    "Recipient",
    "Status",
    "ApprovedBy",
    "ApprovedOn",
    "ApprovalFeedback",
    "PaymentTransactionId",
    "CampaignId",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    'h2222222-2222-2222-2222-222222222222',
    '11111111-1111-1111-1111-111111111111',
    10001, -- Deposit
    3000000.00,
    'Đóng góp cho chiến dịch Tết Nguyên Đán',
    'Gây Quỹ Tết Nguyên Đán 2025',
    NULL,
    11001, -- Approved
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1),
    NOW(),
    'Cảm ơn sự đóng góp',
    'TXN-2025110301',
    'c1111111-1111-1111-1111-111111111111',
    NOW(),
    'System Admin',
    NOW() - INTERVAL '5 days',
    'Trần Thị B',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- History 3: Withdrawal for event
INSERT INTO "FTFundHistories" (
    "Id",
    "FTFundId",
    "MoneyType",
    "MoneyAmount",
    "FundDescription",
    "FundEvent",
    "Recipient",
    "Status",
    "ApprovedBy",
    "ApprovedOn",
    "ApprovalFeedback",
    "PaymentTransactionId",
    "CampaignId",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    'h3333333-3333-3333-3333-333333333333',
    '11111111-1111-1111-1111-111111111111',
    10002, -- Withdrawal
    8000000.00,
    'Chi phí tổ chức sinh nhật gia đình',
    'Sinh nhật lần thứ 70 của Ông Nội',
    'Ban Tổ Chức',
    11001, -- Approved
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1),
    NOW() - INTERVAL '1 day',
    'Đã duyệt chi tiêu cho sự kiện',
    'TXN-2025110201',
    NULL,
    NOW(),
    'System Admin',
    NOW() - INTERVAL '3 days',
    'Lê Văn C',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- History 4: Pending withdrawal request
INSERT INTO "FTFundHistories" (
    "Id",
    "FTFundId",
    "MoneyType",
    "MoneyAmount",
    "FundDescription",
    "FundEvent",
    "Recipient",
    "Status",
    "ApprovedBy",
    "ApprovedOn",
    "ApprovalFeedback",
    "PaymentTransactionId",
    "CampaignId",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    'h4444444-4444-4444-4444-444444444444',
    '22222222-2222-2222-2222-222222222222',
    10002, -- Withdrawal
    5000000.00,
    'Hỗ trợ học phí cho sinh viên năm thứ nhất',
    'Học Bổng Sinh Viên 2025',
    'Nguyễn Văn D',
    11002, -- Pending
    NULL,
    NULL,
    NULL,
    NULL,
    'c3333333-3333-3333-3333-333333333333',
    NOW(),
    'System Admin',
    NOW() - INTERVAL '1 day',
    'Phạm Thị E',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- History 5: Spending from emergency fund
INSERT INTO "FTFundHistories" (
    "Id",
    "FTFundId",
    "MoneyType",
    "MoneyAmount",
    "FundDescription",
    "FundEvent",
    "Recipient",
    "Status",
    "ApprovedBy",
    "ApprovedOn",
    "ApprovalFeedback",
    "PaymentTransactionId",
    "CampaignId",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    'h5555555-5555-5555-5555-555555555555',
    '33333333-3333-3333-3333-333333333333',
    10003, -- Spending
    2000000.00,
    'Chi phí y tế khẩn cấp cho thành viên gia đình',
    'Hỗ trợ y tế khẩn cấp',
    'Hoàng Văn F',
    11001, -- Approved
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1),
    NOW(),
    'Đã duyệt hỗ trợ khẩn cấp',
    'TXN-2025110101',
    NULL,
    NOW(),
    'System Admin',
    NOW() - INTERVAL '7 days',
    'Vũ Thị G',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- History 6: Deposit for scholarship
INSERT INTO "FTFundHistories" (
    "Id",
    "FTFundId",
    "MoneyType",
    "MoneyAmount",
    "FundDescription",
    "FundEvent",
    "Recipient",
    "Status",
    "ApprovedBy",
    "ApprovedOn",
    "ApprovalFeedback",
    "PaymentTransactionId",
    "CampaignId",
    "LastModifiedOn",
    "LastModifiedBy",
    "CreatedOn",
    "CreatedBy",
    "IsDeleted",
    "CreatedByUserId"
) VALUES (
    'h6666666-6666-6666-6666-666666666666',
    '22222222-2222-2222-2222-222222222222',
    10001, -- Deposit
    10000000.00,
    'Đóng góp vào quỹ học bổng',
    'Học Bổng Sinh Viên 2025',
    NULL,
    11001, -- Approved
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1),
    NOW(),
    'Cảm ơn sự đóng góp lớn',
    'TXN-2025103101',
    'c3333333-3333-3333-3333-333333333333',
    NOW(),
    'System Admin',
    NOW() - INTERVAL '4 days',
    'Đỗ Văn H',
    false,
    (SELECT "Id" FROM "ApplicationUser" WHERE "Email" IS NOT NULL LIMIT 1)
);

-- =============================================
-- VERIFICATION QUERIES
-- =============================================

-- Check inserted data
SELECT 'FTFunds Count' as Info, COUNT(*) as Total FROM "FTFunds" WHERE "IsDeleted" = false;
SELECT 'FTFundCampaigns Count' as Info, COUNT(*) as Total FROM "FTFundCampaigns" WHERE "IsDeleted" = false;
SELECT 'FTFundHistories Count' as Info, COUNT(*) as Total FROM "FTFundHistories" WHERE "IsDeleted" = false;

-- View sample data
SELECT "Id", "FundName", "CurrentMoney", "FTId" FROM "FTFunds" WHERE "IsDeleted" = false;
SELECT "Id", "CampaignName", "TargetAmount", "AmountRaised", "Status" FROM "FTFundCampaigns" WHERE "IsDeleted" = false;
SELECT "Id", "MoneyType", "MoneyAmount", "Status", "FundDescription" FROM "FTFundHistories" WHERE "IsDeleted" = false ORDER BY "CreatedOn" DESC;
