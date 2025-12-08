# Test Report: GET /api/ftcampaigndonation/pending/manager/{managerId}

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetPendingDonationsForManager |
| Function Name | Get Pending Donations For Manager |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 10 |
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
| Manager ID is provided in route | O | O | O |
| Manager ID is valid GUID | O | O | O |
| Page parameter is provided (default: 1) | O | O | O |
| PageSize parameter is provided (default: 10) | O | O | O |
| Manager has campaigns | O | - | - |
| Manager has no campaigns | - | O | - |
| Manager ID is valid | O | O | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| DonationService.GetPendingDonationsForManagerAsync() returns paginated donations | O | - | - |
| DonationService.GetPendingDonationsForManagerAsync() returns empty list | - | O | - |
| DonationService.GetPendingDonationsForManagerAsync() throws Exception | - | - | O |
| Paginated response includes Items | O | O | - |
| Paginated response includes TotalCount | O | O | - |
| Paginated response includes Page | O | O | - |
| Paginated response includes PageSize | O | O | - |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| StatusCode: 200 | O | O | - |
| StatusCode: 400 | - | - | O |
| Returns ApiSuccess with paginated donations | O | O | - |
| Returns ApiError with exception message | - | - | O |
| Response includes "Pending donations retrieved successfully" message | O | O | - |
| Response includes Items list | O | O | - |
| Response Items list is not empty | O | - | - |
| Response Items list is empty | - | O | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Server Error | - | - | O |
| Exception thrown | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Retrieved pending donations for manager {ManagerId} successfully | O | O | - |
| No pending donations found for manager {ManagerId} | - | O | - |
| Error retrieving pending donations for manager {ManagerId} | - | - | O |
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

