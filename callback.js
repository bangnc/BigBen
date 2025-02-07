function EventEmitter() {
    this.events = {}; // Lưu trữ các sự kiện và callback tương ứng
  }
  
  // Đăng ký một sự kiện với callback
  EventEmitter.prototype.$on = function(event, callback) {
    if (!this.events[event]) {
      this.events[event] = []; // Nếu chưa có sự kiện này, khởi tạo mảng
    }
    this.events[event].push(callback); // Thêm callback vào danh sách
  };
  
  // Phát một sự kiện, gọi tất cả callback đã đăng ký
  EventEmitter.prototype.$emit = function(event, data) {
    if (this.events[event]) {
      // Lặp qua các callback đã đăng ký và gọi chúng
      this.events[event].forEach(function(callback) {
        callback(data); // Gọi callback với dữ liệu
      });
    }
  };