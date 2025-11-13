# Test Report: DELETE /api/ftmember/{ftMemberId}

## Function Information

| Field | Value |
|-------|-------|
| Function Code | DeleteMember |
| Function Name | Delete Family Tree Member |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 5 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 5 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 1 / 4 / 0 |
| **Total Test Cases** | **5** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | A (Abnormal) | P (Passed) | 06/02 |
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
| ftMemberId is valid GUID | O | O | O | O | O |
| ftMemberId exists in system | O | - | O | O | O |
| ftMemberId does not exist in system | - | O | - | - | - |
| Member can be deleted (no restrictions) | O | - | - | - | - |
| Member has many children | - | - | O | - | - |
| Member has partner relationship | - | - | - | O | - |
| Member is both parent and child | - | - | - | - | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Service.Delete() executes successfully | O | - | - | - | - |
| Service.Delete() throws ArgumentException (Member not found) | - | O | - | - | - |
| Service.Delete() throws ArgumentException (Has many children) | - | - | O | - | - |
| Service.Delete() throws ArgumentException (Has partner relationship) | - | - | - | O | - |
| Service.Delete() throws ArgumentException (Is both parent and child) | - | - | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| StatusCode: 200 | O | - | - | - | - |
| Returns ApiSuccess with message | O | - | - | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Member Not Found | - | O | - | - | - |
| Member Has Many Children | - | - | O | - | - |
| Member Has Partner Relationship | - | - | - | O | - |
| Member Is Both Parent And Child | - | - | - | - | O |
| ArgumentException thrown | - | O | O | O | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Xoá thành viên của gia phả thành công | O | - | - | - | - |
| Không tìm thấy thành viên trong cây gia phả | - | O | - | - | - |
| Thành viên có nhiều con trong cây gia phả, nên không thể xóa | - | - | O | - | - |
| Không thể xóa thành viên vì họ vẫn còn mối quan hệ vợ/chồng | - | - | - | O | - |
| Không thể xóa thành viên vì họ vừa là mối quan hệ cha/mẹ và con | - | - | - | - | O |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | A | P | 06/02 | - |
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



