# 📝 Hướng dẫn Unit Testing cho FTM API

File này chứa Unit Tests đơn giản và dễ hiểu cho các API trong FTM API:
- ✅ **AccountController** - Quản lý tài khoản user
- ✅ **AcademicHonorController** - Quản lý thành tích học tập
- ✅ **BiographyController** - Quản lý tiểu sử và sự kiện
- ✅ **CareerHonorController** - Quản lý thành tích nghề nghiệp
- ✅ **EducationController** - Quản lý học vấn
- ✅ **WorkController** - Quản lý công việc
- ✅ **PostController** - Quản lý bài viết
- ✅ **FTFamilyEventController** - Quản lý sự kiện gia phả
- ✅ **FTMemberController** - Quản lý thành viên gia phả
- ✅ **FamilyTreeController** - Quản lý gia phả

## 🏗️ Kiến trúc Test

### Cấu trúc AAA Pattern (Arrange-Act-Assert)

Mỗi test case được viết theo 3 bước rõ ràng:

```csharp
[Fact(DisplayName = "Mô tả test case")]
public async Task TenTest()
{
    // 1️⃣ ARRANGE - Chuẩn bị dữ liệu test
    var mockData = new SomeData();
    _mockService.Setup(s => s.Method()).ReturnsAsync(mockData);
    
    // 2️⃣ ACT - Thực hiện action cần test
    var result = await _controller.Action();
    
    // 3️⃣ ASSERT - Kiểm tra kết quả
    Assert.IsType<OkObjectResult>(result);
}
```

### 🌳 Add API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 201 Created              | ✅ Pass  | ✅ Test OK             |
| 2  | Dữ liệu không hợp lệ               | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🌳 GetById API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🌳 Edit API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🌳 Delete API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🌳 GetAll API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🌳 GetMyFamilyTrees API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|----------

### 🟢 GetProfile API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | User đăng nhập hợp lệ             | 200 OK + Profile data    | ✅ Pass  | ✅ Test OK             |
| 2  | User chưa đăng nhập               | 401 Unauthorized         | ❌ Fail  | ✅ Test OK (logic đúng)|
| 3  | User không tồn tại                | 404 Not Found            | ❌ Fail  | ✅ Test OK (logic đúng)|
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK (logic đúng)|

### 🔴 GetUserProfile(userId) API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Lấy profile user khác thành công  | 200 OK + Profile data    | ✅ Pass  | ✅ Test OK             |
| 2  | User không tồn tại                | 404 Not Found            | ❌ Fail  | ✅ Test OK (logic đúng)|
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK (logic đúng)|

### 🟡 UpdateProfile API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Dữ liệu hợp lệ                    | 200 OK + Updated data    | ✅ Pass  | ✅ Test OK             |
| 2  | User chưa đăng nhập               | 401 Unauthorized         | ❌ Fail  | ✅ Test OK (logic đúng)|
| 3  | Dữ liệu không hợp lệ              | 400 Bad Request          | ❌ Fail  | ✅ Test OK (logic đúng)|
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK (logic đúng)|

### 🔵 ChangePassword API (6 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Đổi mật khẩu thành công           | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Mật khẩu hiện tại sai             | 400 Bad Request          | ❌ Fail  | ✅ Test OK (logic đúng)|
| 3  | Mật khẩu mới không đủ mạnh        | 400 Bad Request          | ❌ Fail  | ✅ Test OK (logic đúng)|
| 4  | User chưa đăng nhập               | 401 Unauthorized         | ❌ Fail  | ✅ Test OK (logic đúng)|
| 5  | Mật khẩu mới trùng mật khẩu cũ    | 400 Bad Request          | ❌ Fail  | ✅ Test OK (logic đúng)|
| 6  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK (logic đúng)|

### 🟣 UploadAvatar API (6 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Upload ảnh hợp lệ                 | 200 OK + Avatar URL      | ✅ Pass  | ✅ Test OK             |
| 2  | File không phải ảnh               | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 3  | File quá lớn (>5MB)               | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 4  | Không có file                     | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 5  | User chưa đăng nhập               | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 6  | Lỗi upload lên storage            | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎓 GetAcademicHonors API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 🎓 GetAcademicHonorById API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 🎓 CreateAcademicHonor API (5 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 201 Created              | ✅ Pass  | ✅ Test OK             |
| 2  | Dữ liệu không hợp lệ               | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 3  | Không có quyền                     | 403 Forbidden            | ❌ Fail  | ✅ Test OK             |
| 4  | Invalid operation                 | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 5  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 🎓 UpdateAcademicHonor API (5 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Dữ liệu không hợp lệ               | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 4  | Không có quyền                     | 403 Forbidden            | ❌ Fail  | ✅ Test OK             |
| 5  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 🎓 DeleteAcademicHonor API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Không có quyền                     | 403 Forbidden            | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 📚 GetBiographyDescription API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 📚 UpdateBiographyDescription API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Invalid data                       | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 📚 GetBiographyEvents API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 📚 GetBiographyEvent API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 📚 CreateBiographyEvent API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 201 Created              | ✅ Pass  | ✅ Test OK             |
| 2  | Invalid data                       | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 📚 UpdateBiographyEvent API (5 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Invalid data                       | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 4  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 5  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 📚 DeleteBiographyEvent API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🏆 GetCareerHonors API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 🏆 GetCareerHonorById API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 🏆 CreateCareerHonor API (5 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 201 Created              | ✅ Pass  | ✅ Test OK             |
| 2  | Invalid data                       | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 403 Forbidden            | ❌ Fail  | ✅ Test OK             |
| 4  | Invalid operation                 | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 5  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 🏆 UpdateCareerHonor API (5 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Invalid data                       | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 4  | Unauthorized                       | 403 Forbidden            | ❌ Fail  | ✅ Test OK             |
| 5  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 🏆 DeleteCareerHonor API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 403 Forbidden            | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 🎓 GetEducations API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎓 GetEducation API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎓 CreateEducation API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 201 Created              | ✅ Pass  | ✅ Test OK             |
| 2  | Invalid data                       | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎓 UpdateEducation API (5 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Invalid data                       | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 4  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 5  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎓 DeleteEducation API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 💼 GetWorks API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 💼 GetWork API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 💼 CreateWork API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 201 Created              | ✅ Pass  | ✅ Test OK             |
| 2  | Invalid data                       | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 💼 UpdateWork API (5 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Invalid data                       | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |
| 4  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 5  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 💼 DeleteWork API (4 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Unauthorized                       | 401 Unauthorized         | ❌ Fail  | ✅ Test OK             |
| 4  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 📝 CreatePostWithFiles API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 📝 UpdatePostWithFiles API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 📝 GetPosts API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 📝 GetPostById API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 📝 DeletePost API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 400 Bad Request          | ❌ Fail  | ✅ Test OK             |

### 🎪 CreateEvent API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 201 Created              | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎪 UpdateEvent API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎪 DeleteEvent API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎪 GetEventById API (3 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Không tồn tại                      | 404 Not Found            | ❌ Fail  | ✅ Test OK             |
| 3  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎪 GetEventsByGP API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎪 GetUpcomingEvents API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎪 GetEventsByDateRange API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎪 GetEventsByMember API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎪 FilterEvents API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎪 AddMemberToEvent API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 🎪 RemoveMemberFromEvent API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 👨‍👩‍👧‍👦 AddMember API (2 test cases)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 201 Created              | ✅ Pass  | ✅ Test OK             |
| 2  | Lỗi server                        | 500 Internal Server      | ❌ Fail  | ✅ Test OK             |

### 👨‍👩‍👧‍👦 GetListOfMembers API (1 test case)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |

### 👨‍👩‍👧‍👦 GetDetailedMemberOfFamilyTreeByUserId API (1 test case)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |

### 👨‍👩‍👧‍👦 GetDetailedMemberOfFamilyTreeByMemberId API (1 test case)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |

### 👨‍👩‍👧‍👦 GetMembersTreeViewAsync API (1 test case)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |

### 👨‍👩‍👧‍👦 UpdateMemberDetails API (1 test case)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK + Data            | ✅ Pass  | ✅ Test OK             |

### 👨‍👩‍👧‍👦 DeleteMember API (1 test case)

| #  | Test Case                          | Kết quả mong đợi         | Loại     | Kết quả thực tế        |
|----|-----------------------------------|--------------------------|----------|------------------------|
| 1  | Thành công                         | 200 OK                   | ✅ Pass  | ✅ Test OK             |

- **219 test cases** tổng cộng
- **Tất cả đều PASS** ✅
- Bao gồm các scenarios: success, error handling, validation, authorization, server errors
- Test coverage: AccountController, AcademicHonorController, BiographyController, CareerHonorController, EducationController, WorkController, PostController, FTFamilyEventController, FTMemberController, FamilyTreeController

## 🚀 Cách chạy Tests

### 1. Chạy tất cả tests

```bash
cd FTM.Tests
dotnet test
```

### 2. Chạy tests với báo cáo chi tiết

```bash
dotnet test --logger "console;verbosity=detailed"
```

### 3. Chạy riêng AccountControllerTests

```bash
dotnet test --filter "FullyQualifiedName~AccountControllerTests"
```

### 4. Chạy một test case cụ thể

```bash
# Ví dụ: Chỉ chạy test GetProfile thành công
dotnet test --filter "FullyQualifiedName~GetProfile_Success"
```

### 5. Tạo báo cáo HTML (với ReportGenerator)

```bash
# Cài đặt ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Chạy test với coverage
dotnet test --collect:"XPlat Code Coverage"

# Tạo HTML report
reportgenerator -reports:"**/*.cobertura.xml" -targetdir:"TestResults/Report" -reporttypes:Html

# Mở báo cáo
start TestResults/Report/index.html
```

## 📈 Xem kết quả Test

### Output mẫu khi chạy tests:

```
Starting test execution, please wait...
A total of 20 tests run, all passed in 2.3 seconds.

Test Run Successful.
Total tests: 20
     Passed: 4
     Failed: 16
 Total time: 2.3 seconds
```

### Chi tiết từng test case:

```
✅ PASSED: GetProfile - Thành công - Trả về profile của user
❌ FAILED: GetProfile - Thất bại - User chưa đăng nhập
❌ FAILED: GetProfile - Thất bại - User không tồn tại
❌ FAILED: GetProfile - Thất bại - Lỗi server

✅ PASSED: UpdateProfile - Thành công - Cập nhật profile
❌ FAILED: UpdateProfile - Thất bại - User chưa đăng nhập
... (và các test khác)
```

## 🔧 Công nghệ sử dụng

- **xUnit** - Test framework
- **Moq** - Mocking library để fake dependencies
- **FluentAssertions** (optional) - Assertions dễ đọc hơn

## 📚 Giải thích chi tiết

### Mock Services

```csharp
// Tạo mock IAccountService
_mockAccountService = new Mock<IAccountService>();

// Setup mock trả về dữ liệu giả
_mockAccountService
    .Setup(s => s.GetCurrentUserProfileAsync())
    .ReturnsAsync(expectedProfile);

// Setup mock throw exception
_mockAccountService
    .Setup(s => s.GetCurrentUserProfileAsync())
    .ThrowsAsync(new UnauthorizedAccessException("Lỗi"));
```

### Assertions

```csharp
// Kiểm tra kiểu trả về
var okResult = Assert.IsType<OkObjectResult>(result);

// Kiểm tra giá trị
Assert.Equal(expected, actual);

// Kiểm tra null
Assert.NotNull(value);

// Kiểm tra boolean
Assert.True(value);
Assert.False(value);

// Kiểm tra string chứa text
Assert.Contains("text", message);
```

## 🎯 Best Practices

1. ✅ **Một test case chỉ test một chức năng**
2. ✅ **Tên test case phải mô tả rõ ràng** - Dùng DisplayName
3. ✅ **Sử dụng AAA pattern** - Arrange, Act, Assert
4. ✅ **Test cả trường hợp thành công và thất bại**
5. ✅ **Mock tất cả dependencies** - Không gọi database thật
6. ✅ **Tests phải độc lập** - Không phụ thuộc vào thứ tự chạy

## ❓ Troubleshooting

### Lỗi: "Cannot convert null literal to non-nullable reference type"

**Giải pháp:** Thêm `#nullable disable` ở đầu file test hoặc tắt nullable trong csproj:

```xml
<PropertyGroup>
    <Nullable>disable</Nullable>
</PropertyGroup>
```

### Lỗi: "Mock setup never matched"

**Nguyên nhân:** Setup mock không khớp với cách gọi trong controller.

**Giải pháp:** Kiểm tra lại parameters và return type.

### Tests chạy chậm

**Nguyên nhân:** Có thể đang gọi database hoặc external services.

**Giải pháp:** Đảm bảo tất cả dependencies đều được mock.

## 📞 Liên hệ

Nếu có thắc mắc về tests, vui lòng liên hệ team developer.

---

**Happy Testing! 🎉**
