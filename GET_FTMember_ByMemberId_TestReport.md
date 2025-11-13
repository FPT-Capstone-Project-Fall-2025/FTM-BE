# Test Report: GET /api/ftmember/{ftid}/get-by-memberid

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetDetailedMemberOfFamilyTreeByMemberId |
| Function Name | Get Detailed Member of Family Tree by MemberId |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 5 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 2 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 1 / 1 / 0 |
| **Total Test Cases** | **2** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| Access to system | O | O |
| Internet connection must be stable | O | O |
| Users have logged in | O | O |
| User has proper permission | O | O |

### Request

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| ftId is valid GUID | O | O |
| memberId is valid GUID | O | O |
| memberId exists in system | O | - |
| memberId does not exist in system | - | O |
| Member exists in family tree | O | - |
| Member does not exist in family tree | - | O |

### Service

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| Service.GetByMemberId() executes successfully | O | - |
| Service.GetByMemberId() throws ArgumentException (Member not found) | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| StatusCode: 200 | O | - |
| Returns ApiSuccess with message | O | - |
| Returns FTMemberDetailsDto | O | - |

### Exception

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| Member Not Found | - | O |
| ArgumentException thrown | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| Lấy thông tin của thành viên gia phả thành công | O | - |
| Không tìm thấy thành viên gia phả | - | O |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | A | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed



