# Test Report: POST /api/ftcampaigndonation/campaign/{campaignId}/donate

## Function Information

| Field | Value |
|-------|-------|
| Function Code | DonateToCampaign |
| Function Name | Donate to Campaign |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 69 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 4 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 2 / 2 / 0 |
| **Total Test Cases** | **4** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | N (Normal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID04 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Access to system | O | O | O | O |
| Internet connection must be stable | O | O | O | O |
| Users have logged in | O | O | O | O |
| User has proper permission | O | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Campaign ID is provided in route | O | O | O | O |
| Campaign ID is valid GUID | O | O | O | O |
| Request body is provided | O | O | O | O |
| MemberId is provided (optional) | O | O | O | O |
| DonorName is provided | O | O | O | O |
| Amount is provided | O | O | O | O |
| PaymentMethod is Cash | O | - | - | - |
| PaymentMethod is BankTransfer | - | O | - | - |
| ProofImages are provided (for Cash) | O | - | - | - |
| Campaign exists | O | O | - | O |
| Campaign does not exist | - | - | O | - |
| Campaign has bank account info | - | O | - | - |
| Campaign does not have bank account info | - | - | - | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| DonationService.GetCampaignForDonationAsync() returns campaign | O | O | - | O |
| DonationService.GetCampaignForDonationAsync() returns null | - | - | O | - |
| PayOSService.CreateCampaignDonationPaymentAsync() executes (for BankTransfer) | - | O | - | - |
| DonationService.AddAsync() executes successfully | O | O | - | - |
| DonationService.AddAsync() creates donation with Pending status | O | O | - | - |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| StatusCode: 200 | O | O | - | - |
| StatusCode: 404 | - | - | O | - |
| StatusCode: 400 | - | - | - | O |
| Returns ApiSuccess with donation info | O | O | - | - |
| Returns ApiError with "Campaign not found" | - | - | O | - |
| Returns ApiError with bank account message | - | - | - | O |
| Response includes DonationId | O | O | - | - |
| Response includes QrCodeUrl (for BankTransfer) | - | O | - | - |
| Response includes RequiresManualConfirmation | O | - | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Campaign Not Found | - | - | O | - |
| Bank Account Information Missing | - | - | - | O |
| Campaign has not set up bank account information | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Created donation {DonationId} for campaign {CampaignId} | O | O | - | - |
| Campaign not found | - | - | O | - |
| Campaign has not set up bank account information | - | - | - | O |
| Cash donation recorded. Waiting for manager confirmation. | O | - | - | - |
| Please scan QR code to complete payment | - | O | - | - |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |
| UTCID04 | - |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | N | P | 06/02 | - |
| UTCID03 | A | P | 06/02 | - |
| UTCID04 | A | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

