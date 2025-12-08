# Test Report: POST /api/donations/{donationId}/upload-proof

## Function Information

| Field | Value |
|-------|-------|
| Function Code | UploadProofImages |
| Function Name | Upload Proof Images for Donation |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 50 |
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
| donationId is valid GUID | O | O | O | O |
| images are provided | O | O | O | - |
| images list is empty | - | - | - | O |
| images contain valid files | O | - | - | - |
| Donation exists | O | - | - | O |
| Donation does not exist | - | O | - | - |
| Donation status is Pending | O | - | - | O |
| Donation status is not Pending | - | - | O | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Service.GetByIdAsync() returns donation | O | - | O | O |
| Service.GetByIdAsync() returns null | - | O | - | - |
| BlobStorageService.UploadFileAsync() executes | O | - | - | - |
| Service.UpdateDonationAsync() executes | O | - | - | - |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| StatusCode: 200 | O | - | - | - |
| StatusCode: 404 | - | O | - | - |
| StatusCode: 400 | - | - | O | O |
| Returns ApiSuccess with image URLs | O | - | - | - |
| Returns ApiError with message | - | O | O | O |
| ImageUrls array contains uploaded URLs | O | - | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Donation Not Found | - | O | - | - |
| Donation Not Pending | - | - | O | - |
| No Images Provided | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Uploaded {Count} proof images for donation {DonationId} | O | - | - | - |
| Error uploading proof images for donation | - | - | - | O |
| Donation not found | - | O | - | - |
| Can only upload proof for pending donations | - | - | O | - |
| No images provided | - | - | - | O |

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

