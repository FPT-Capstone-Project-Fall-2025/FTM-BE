# Test Report: GET /api/ftfamilyevent/{year}

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetEventsGroupedByYear |
| Function Name | Get Events Grouped by Year |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 13 |
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
| year is valid integer | O | O | O |
| ftId is provided as query parameter | O | O | O |
| Events exist for year | O | - | - |
| No events exist for year | - | O | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Service.GetEventsGroupedByYearAsync() returns events | O | - | - |
| Service.GetEventsGroupedByYearAsync() returns empty list | - | O | - |
| Service.GetEventsGroupedByYearAsync() throws Exception | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| StatusCode: 200 | O | O | - |
| StatusCode: 400 | - | - | O |
| Returns ApiSuccess with events | O | O | - |
| Returns ApiError with message | - | - | O |
| Events list contains data | O | - | - |
| Events list is empty | - | O | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Server Error | - | - | O |
| Exception thrown | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Events in year successfully | O | O | - |
| Server error message | - | - | O |

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



