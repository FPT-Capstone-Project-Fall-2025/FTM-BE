# Test Report: POST /api/ftfamilyevent

## Function Information

| Field | Value |
|-------|-------|
| Function Code | CreateEvent |
| Function Name | Create Family Tree Event |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 14 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 4 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 1 / 3 / 0 |
| **Total Test Cases** | **4** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | A (Abnormal) | P (Passed) | 06/02 |
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
| Request data is valid | O | O | O | - |
| Request data is invalid | - | - | - | - |
| Name is provided | O | O | O | O |
| EventType is provided | O | O | O | O |
| StartTime is provided | O | O | O | O |
| FTId is provided | O | O | O | O |
| FTId exists in system | O | - | O | O |
| FTId does not exist in system | - | O | - | - |
| RecurrenceType is valid (0-3) | O | O | - | O |
| RecurrenceType is invalid | - | - | O | - |
| StartTime is before EndTime | O | O | O | - |
| StartTime is after EndTime | - | - | - | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Service.CreateEventAsync() executes successfully | O | - | - | - |
| Service.CreateEventAsync() throws Exception (Family tree not found) | - | O | - | - |
| Service.CreateEventAsync() throws Exception (Invalid recurrence type) | - | - | O | - |
| Service.CreateEventAsync() throws Exception (Invalid time range) | - | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| StatusCode: 200 | O | - | - | - |
| StatusCode: 400 | - | O | O | O |
| Returns ApiSuccess with message | O | - | - | - |
| Returns ApiError with message | - | O | O | O |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Family Tree Not Found | - | O | - | - |
| Invalid Recurrence Type | - | - | O | - |
| Invalid Time Range | - | - | - | O |
| Exception thrown | - | O | O | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Event created successfully | O | - | - | - |
| Family tree with ID not found | - | O | - | - |
| Invalid recurrence type | - | - | O | - |
| Start time must be before end time | - | - | - | O |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | A | P | 06/02 | - |
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



