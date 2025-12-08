# Test Report: GET /api/fund-expenses/pending

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetPendingExpenses |
| Function Name | Get Pending Expenses |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 38 |
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
| fundId is optional (can be null) | O | O | O |
| fundId is provided | O | - | - |
| fundId is null | - | O | O |
| page parameter is provided | O | O | O |
| pageSize parameter is provided | O | O | O |
| Pending expenses exist | O | - | - |
| No pending expenses exist | - | O | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| ExpenseService.GetPendingExpensesAsync() returns expenses | O | - | - |
| ExpenseService.GetPendingExpensesAsync() returns empty list | - | O | - |
| ExpenseService.GetPendingExpensesAsync() throws Exception | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| StatusCode: 200 | O | O | - |
| StatusCode: 500 | - | - | O |
| Returns ApiSuccess with paginated expenses | O | O | - |
| Returns ApiError with message | - | - | O |
| Expenses list contains data | O | - | - |
| Expenses list is empty | - | O | - |
| Pagination info is included | O | O | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Server Error | - | - | O |
| Exception thrown | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Pending expenses retrieved successfully | O | O | - |
| Error getting pending expenses | - | - | O |
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

