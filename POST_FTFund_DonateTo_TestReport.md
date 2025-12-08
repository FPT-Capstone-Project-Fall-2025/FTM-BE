# Test Report: POST /api/funds/{fundId}/donate

## Function Information

| Field | Value |
|-------|-------|
| Function Code | DonateTo |
| Function Name | Create Donation to Fund |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 83 |
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
| fundId is valid GUID | O | O | O | O |
| Request body is provided | O | O | O | O |
| Amount is provided | O | O | O | O |
| PaymentMethod is provided | O | O | O | O |
| PaymentMethod = Cash | O | - | - | - |
| PaymentMethod = BankTransfer | - | O | - | O |
| Fund exists | O | O | - | O |
| Fund does not exist | - | - | O | - |
| Fund has bank information | - | O | - | - |
| Fund missing bank information | - | - | - | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Service.GetByIdAsync() returns fund | O | O | - | O |
| Service.GetByIdAsync() returns null | - | - | O | - |
| Service.CreateDonationAsync() executes successfully | O | O | - | - |
| PaymentService.GenerateOrderCode() executes | - | O | - | - |
| PaymentService.GenerateVietQRUrl() executes | - | O | - | - |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| StatusCode: 200 | O | O | - | - |
| StatusCode: 404 | - | - | O | - |
| StatusCode: 400 | - | - | - | O |
| Returns ApiSuccess with donation info | O | O | - | - |
| Returns ApiError with message | - | - | O | O |
| QRCodeUrl is generated (BankTransfer) | - | O | - | - |
| RequiresManualConfirmation = true (Cash) | O | - | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Fund Not Found | - | - | O | - |
| Missing Bank Information | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Created donation {DonationId} for fund {FundId} | O | O | - | - |
| Created VietQR for fund donation | - | O | - | - |
| Error creating donation for fund | - | - | O | O |
| Fund has not set up bank account information | - | - | - | O |

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

