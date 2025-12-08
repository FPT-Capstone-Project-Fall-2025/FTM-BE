# Test Report: PUT /api/ftcampaignexpense/{id}/approve

## Function Information

| Field | Value |
|-------|-------|
| Function Code | ApproveExpense |
| Function Name | Approve Campaign Expense |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 51 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 2 / 5 / 0 |
| **Total Test Cases** | **7** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | N (Normal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID04 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID05 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID06 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID07 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|-----------|---------|---------|---------|---------|---------|---------|---------|
| Access to system | O | O | O | O | O | O | O |
| Internet connection must be stable | O | O | O | O | O | O | O |
| Users have logged in | O | O | O | O | O | O | O |
| User has proper permission | O | O | O | O | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|-----------|---------|---------|---------|---------|---------|---------|---------|
| Expense ID is provided in route | O | O | O | O | O | O | O |
| Expense ID is valid GUID | O | O | O | O | O | O | O |
| Request body is provided | O | O | O | O | O | O | O |
| ApproverId is provided | O | O | O | O | O | O | O |
| ApprovalNotes is provided (optional) | O | O | O | O | O | O | O |
| PaymentProofImage is provided | O | - | - | - | - | O | - |
| PaymentProofImage is not provided | - | O | - | - | - | - | - |
| Expense exists | O | O | - | O | O | O | O |
| Expense does not exist | - | - | O | - | - | - | - |
| Campaign exists | O | O | - | - | O | O | O |
| Campaign does not exist | - | - | - | O | - | - | - |
| ApproverId matches Campaign Manager ID | O | O | - | - | - | O | O |
| ApproverId does not match Campaign Manager ID | - | - | - | - | O | - | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|-----------|---------|---------|---------|---------|---------|---------|---------|
| ExpenseService.GetByIdAsync() returns expense | O | O | - | O | O | O | O |
| ExpenseService.GetByIdAsync() returns null | - | - | O | - | - | - | - |
| CampaignService.GetByIdAsync() returns campaign | O | O | - | - | O | O | O |
| CampaignService.GetByIdAsync() returns null | - | - | - | O | - | - | - |
| BlobStorageService.UploadFileAsync() executes (if image provided) | O | - | - | - | - | - | - |
| BlobStorageService.UploadFileAsync() throws Exception | - | - | - | - | - | O | - |
| ExpenseService.ApproveExpenseAsync() executes successfully | O | O | - | - | - | - | - |
| ExpenseService.ApproveExpenseAsync() throws Exception | - | - | - | - | - | - | O |
| Expense status is updated to Approved | O | O | - | - | - | - | - |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|-----------|---------|---------|---------|---------|---------|---------|---------|
| StatusCode: 200 | O | O | - | - | - | - | - |
| StatusCode: 400 | - | - | - | - | - | O | O |
| StatusCode: 403 | - | - | - | - | O | - | - |
| StatusCode: 404 | - | - | O | O | - | - | - |
| Returns ApiSuccess with expense info | O | O | - | - | - | - | - |
| Returns ApiError with "Expense not found" | - | - | O | - | - | - | - |
| Returns ApiError with "Campaign not found" | - | - | - | O | - | - | - |
| Returns ApiError with authorization message | - | - | - | - | O | - | - |
| Returns ApiError with exception message | - | - | - | - | - | O | O |
| Response includes ExpenseId | O | O | - | - | - | - | - |
| Response includes Status | O | O | - | - | - | - | - |
| Response includes PaymentProofUrl | O | - | - | - | - | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|-----------|---------|---------|---------|---------|---------|---------|---------|
| Expense Not Found | - | - | O | - | - | - | - |
| Campaign Not Found | - | - | - | O | - | - | - |
| Unauthorized (Not Campaign Manager) | - | - | - | - | O | - | - |
| Blob Storage Error | - | - | - | - | - | O | - |
| Server Error | - | - | - | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|-----------|---------|---------|---------|---------|---------|---------|---------|
| Approved expense {ExpenseId} successfully | O | O | - | - | - | - | - |
| Expense not found | - | - | O | - | - | - | - |
| Campaign not found | - | - | - | O | - | - | - |
| Only the Campaign Manager can approve expenses | - | - | - | - | O | - | - |
| Blob storage error | - | - | - | - | - | O | - |
| Error approving expense {ExpenseId} | - | - | - | - | - | - | O |
| Server error | - | - | - | - | - | - | O |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |
| UTCID04 | - |
| UTCID05 | - |
| UTCID06 | - |
| UTCID07 | - |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | N | P | 06/02 | - |
| UTCID03 | A | P | 06/02 | - |
| UTCID04 | A | P | 06/02 | - |
| UTCID05 | A | P | 06/02 | - |
| UTCID06 | A | P | 06/02 | - |
| UTCID07 | A | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

