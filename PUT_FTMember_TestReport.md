# Test Report: PUT /api/ftmember/{ftId}

## Function Information

| Field | Value |
|-------|-------|
| Function Code | UpdateFTMember |
| Function Name | Update Family Tree Member |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 11 |
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
| ftId is valid GUID | O | O | O | O | O |
| Request data is valid | O | - | O | O | O |
| Request data is invalid (ModelState Invalid) | - | O | - | - | - |
| ftMemberId is provided and valid | O | - | - | - | - |
| ftMemberId is empty or invalid | - | O | O | O | O |
| ftMemberId exists in system | O | - | - | - | - |
| ftMemberId does not exist in system | - | - | O | O | O |
| UserId is provided and exists | - | - | - | - | - |
| UserId is provided but not exists | - | - | - | O | - |
| UserId is provided but already connected | - | - | - | - | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Service.UpdateDetailsByMemberId() executes successfully | O | - | - | - | - |
| Service.UpdateDetailsByMemberId() throws ArgumentException (Member not found) | - | - | O | - | - |
| Service.UpdateDetailsByMemberId() throws ArgumentException (User not found) | - | - | - | O | - |
| Service.UpdateDetailsByMemberId() throws ArgumentException (User already connected) | - | - | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| StatusCode: 200 | O | - | - | - | - |
| StatusCode: 400 | - | O | - | - | - |
| Returns ApiSuccess with message | O | - | - | - | - |
| Returns ApiError with message | - | O | - | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Invalid ModelState | - | O | - | - | - |
| Member Not Found | - | - | O | - | - |
| User Not Found | - | - | - | O | - |
| User Already Connected | - | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Cập nhật thông tin thành viên thành công | O | - | - | - | - |
| Validation error message | - | O | - | - | - |
| Thành viên không tồn tại trong gia phả | - | - | O | - | - |
| Người được mời không tồn tại trong hệ thống | - | - | - | O | - |
| Nguời dùng đã được liên kết với một thành viên khác trong gia phả | - | - | - | - | O |

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

