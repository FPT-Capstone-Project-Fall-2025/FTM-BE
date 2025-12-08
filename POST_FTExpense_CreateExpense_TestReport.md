# Test Report: POST /api/fund-expenses

## Function Information

| Field | Value |
|-------|-------|
| Function Code | CreateExpense |
| Function Name | Create Fund Expense |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 46 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 3 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 2 / 1 / 0 |
| **Total Test Cases** | **3** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | N (Normal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Access to system | O | O | O |
| Internet connection must be stable | O | O | O |
| Users have logged in | O | O | O |
| User has proper permission | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Request body is provided | O | O | O |
| FundId is valid GUID | O | O | O |
| Amount is provided | O | O | O |
| Description is provided | O | O | O |
| ReceiptImages are provided | O | - | - |
| ReceiptImages are not provided | - | O | - |
| ReceiptImages contain valid files | O | - | - |
| Request data is valid | O | O | - |
| Request data causes error | - | - | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| BlobStorageService.UploadFileAsync() executes (if images provided) | O | - | - |
| ExpenseService.CreateExpenseAsync() executes successfully | O | O | - |
| ExpenseService.CreateExpenseAsync() throws Exception | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| StatusCode: 200 | O | O | - |
| StatusCode: 500 | - | - | O |
| Returns ApiSuccess with ExpenseId | O | O | - |
| Returns ApiError with message | - | - | O |
| ReceiptCount is included in response | O | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Server Error | - | - | O |
| Exception thrown | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Created expense {ExpenseId} for fund {FundId} | O | O | - |
| Error creating expense for fund | - | - | O |
| Server error | - | - | O |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | N | P | 06/02 | - |
| UTCID03 | A | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

