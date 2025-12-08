# Test Report: POST /api/fund-expenses/{expenseId}/approve

## Function Information

| Field | Value |
|-------|-------|
| Function Code | ApproveExpense |
| Function Name | Approve Fund Expense |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 47 |
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
| User claims are available | O | O | O | O |
| X-Ftid header is provided | O | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| expenseId is valid GUID | O | O | O | O |
| Request body is provided | O | O | O | O |
| PaymentProof is provided | O | - | - | - |
| PaymentProof is not provided | - | O | - | - |
| Notes is provided | O | O | O | O |
| User ID from claims is valid | O | O | O | O |
| FTId from header is valid | O | O | O | O |
| Member exists in family tree | O | O | - | O |
| Member does not exist in family tree | - | - | O | - |
| Expense exists | O | O | - | O |
| Expense status is Pending | O | O | - | - |
| Expense status is not Pending | - | - | - | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| MemberService.GetByUserId() returns member | O | O | - | O |
| MemberService.GetByUserId() returns null | - | - | O | - |
| BlobStorageService.UploadFileAsync() executes (if proof provided) | O | - | - | - |
| ExpenseService.ApproveExpenseAsync() executes successfully | O | O | - | - |
| ExpenseService.ApproveExpenseAsync() throws InvalidOperationException | - | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| StatusCode: 200 | O | O | - | - |
| StatusCode: 400 | - | - | O | O |
| Returns ApiSuccess with expense info | O | O | - | - |
| Returns ApiError with message | - | - | O | O |
| Expense status is Approved | O | O | - | - |
| PaymentProof URL is included | O | - | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Member Not Found | - | - | O | - |
| Expense Not Pending | - | - | - | O |
| InvalidOperationException | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Approved expense {ExpenseId} by member {MemberId} | O | O | - | - |
| Error approving expense {ExpenseId} | - | - | - | - |
| Member not found in this family tree | - | - | O | - |
| Can only approve pending expenses | - | - | - | O |

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

