# Test Report: POST /api/ftfamilyevent/filter

## Function Information

| Field | Value |
|-------|-------|
| Function Code | FilterEvents |
| Function Name | Filter Family Tree Events |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 13 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 4 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 2 / 1 / 1 |
| **Total Test Cases** | **4** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | N (Normal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID04 | B (Boundary) | P (Passed) | 06/02 |

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
| Request body is provided | O | O | O | O |
| FTId is provided | O | O | O | O |
| Filter criteria are provided | O | O | O | O |
| Single filter criteria | O | O | O | - |
| Multiple filter criteria | - | - | - | O |
| Events match filter criteria | O | - | - | O |
| No events match filter criteria | - | O | - | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Service.FilterEventsAsync() returns events | O | - | - | O |
| Service.FilterEventsAsync() returns empty list | - | O | - | - |
| Service.FilterEventsAsync() throws Exception | - | - | O | - |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| StatusCode: 200 | O | O | - | O |
| StatusCode: 400 | - | - | O | - |
| Returns ApiSuccess with events | O | O | - | O |
| Returns ApiError with message | - | - | O | - |
| Events list contains data | O | - | - | O |
| Events list is empty | - | O | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Server Error | - | - | O | - |
| Exception thrown | - | - | O | - |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Events filtered successfully | O | O | - | O |
| Server error message | - | - | O | - |

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
| UTCID04 | B | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

