# Test Report: POST /api/ftfamilyevent/{eventId}/add-member/{memberId}

## Function Information

| Field | Value |
|-------|-------|
| Function Code | AddMemberToEvent |
| Function Name | Add Member to Event |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 15 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 3 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 1 / 2 / 0 |
| **Total Test Cases** | **3** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Access to system | O | O | O |
| Internet connection must be stable | O | O | O |
| Users have logged in | O | O | O |
| User has proper permission | O | O | O |
| User is authenticated | O | O | O |
| User has valid claims | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| EventId is provided | O | O | O |
| MemberId is provided | O | O | O |
| EventId is valid GUID | O | O | O |
| MemberId is valid GUID | O | O | O |
| Member is not in event | O | - | - |
| Member is already in event | - | O | - |
| Event exists | O | O | O |
| Event does not exist | - | O | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Service.AddMemberToEventAsync() executes successfully | O | - | - |
| Service returns true | O | - | - |
| Service returns false | - | O | - |
| Service throws Exception | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| StatusCode: 200 | O | - | - |
| StatusCode: 400 | - | O | O |
| Returns ApiSuccess with message | O | - | - |
| Returns ApiError with message | - | O | O |
| Returns true | O | - | - |
| Error message: "Member already in event or event not found" | - | O | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Service Exception | - | - | O |
| Server error | - | - | O |
| Member already in event | - | O | - |
| Event not found | - | O | - |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Member added to event successfully | O | - | - |
| Member already in event or event not found | - | O | - |
| Exception message | - | - | O |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed



