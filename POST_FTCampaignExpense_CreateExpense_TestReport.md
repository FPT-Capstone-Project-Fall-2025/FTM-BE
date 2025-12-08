# Test Report: POST /api/ftcampaignexpense

## Function Information

| Field | Value |
|-------|-------|
| Function Code | CreateExpense |
| Function Name | Create Campaign Expense |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 42 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 5 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 2 / 3 / 0 |
| **Total Test Cases** | **5** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | N (Normal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID04 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID05 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Access to system | O | O | O | O | O |
| Internet connection must be stable | O | O | O | O | O |
| Users have logged in | O | O | O | O | O |
| User has proper permission | O | O | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Request body is provided | O | O | O | O | O |
| CampaignId is provided | O | O | O | O | O |
| CampaignId is valid GUID | O | O | O | O | O |
| Amount is provided | O | O | O | O | O |
| Description is provided | O | O | O | O | O |
| Category is provided | O | O | O | O | O |
| AuthorizedBy is provided | O | O | O | O | O |
| ReceiptImages are provided | O | - | - | O | - |
| ReceiptImages are not provided | - | O | - | - | - |
| ReceiptImages contain valid files | O | - | - | - | - |
| Campaign exists | O | O | - | O | O |
| Campaign does not exist | - | - | O | - | - |
| Request data is valid | O | O | - | - | - |
| Request data causes error | - | - | O | O | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| BlobStorageService.UploadFileAsync() executes (if images provided) | O | - | - | - | - |
| BlobStorageService.UploadFileAsync() throws Exception | - | - | - | O | - |
| ExpenseService.AddAsync() executes successfully | O | O | - | - | - |
| ExpenseService.AddAsync() throws InvalidOperationException (Campaign not found) | - | - | O | - | - |
| ExpenseService.AddAsync() throws Exception | - | - | - | - | O |
| Expense is created with Pending status | O | O | - | - | - |
| Receipt URLs are stored in expense | O | - | - | - | - |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| StatusCode: 200 | O | O | - | - | - |
| StatusCode: 400 | - | - | O | O | O |
| Returns ApiSuccess with expense info | O | O | - | - | - |
| Returns ApiError with exception message | - | - | O | O | O |
| Response includes ExpenseId | O | O | - | - | - |
| Response includes Amount | O | O | - | - | - |
| Response includes Status | O | O | - | - | - |
| Response includes ReceiptCount | O | - | - | - | - |
| Response includes ReceiptUrls | O | - | - | - | - |
| Response message includes "pending approval" | O | O | - | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Campaign Not Found | - | - | O | - | - |
| Blob Storage Error | - | - | - | O | - |
| Server Error | - | - | - | - | O |
| InvalidOperationException | - | - | O | - | - |
| Exception thrown | - | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Created expense {ExpenseId} for campaign {CampaignId} | O | O | - | - | - |
| Campaign not found | - | - | O | - | - |
| Blob storage error | - | - | - | O | - |
| Error creating expense for campaign | - | - | - | - | O |
| Server error | - | - | - | - | O |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |
| UTCID04 | - |
| UTCID05 | - |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | N | P | 06/02 | - |
| UTCID03 | A | P | 06/02 | - |
| UTCID04 | A | P | 06/02 | - |
| UTCID05 | A | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

