// Khởi tạo ứng dụng AngularJS
var app = angular.module('myApp', []);

// Dịch vụ để gọi API backend
app.service('AuthService', function($http) {
    var apiUrl = 'https://localhost:7174'; // Địa chỉ backend của bạn
    $http.defaults.withCredentials = true;
    
    this.login = function(username, password) {
        var loginData = { username: username, password: password };
        return $http.post(apiUrl + '/Account/Login', loginData);
    };

    this.logout = function() {
        return $http.post(apiUrl + '/Account/Logout');
    };

    this.loadData = function() {
        return $http.get(apiUrl + '/Account/LoadData')
    }
});

app.service('SignalRService', function($q) {
    var connection;
    var listeners = []; // Danh sách các hàm lắng nghe
     this.status = false

    // Kết nối đến SignalR Hub
    this.connect = function() {
        connection = new signalR.HubConnectionBuilder()
            .withUrl('https://localhost:7174/chatHub', {
                withCredentials: true
            }) // URL của SignalR Hub
            .build();

        connection.start()
            .then(function() {
                console.log("SignalR connected successfully");
                this.start = true
            })
            .catch(function(err) {
                console.error("SignalR connection failed:", err.toString());
                this.start = false
            });

        // Nhận tin nhắn từ server
        connection.on("ReceiveMessage", function(user, message) {
            listeners.forEach(function(listener) {
                listener(user, message);
            });
        });
    };

    // Đăng ký listener
    this.onMessageReceived = function(callback) {
        listeners.push(callback);
    };

    // Gửi tin nhắn lên server
    this.sendMessage = function(user, message) {
        connection.invoke("SendMessage", user, message).catch(function(err) {
            console.error(err.toString());
        });
    };
});

// Controller để xử lý logic đăng nhập và đăng xuất
app.controller('LoginController', function($scope, AuthService) {
    $scope.username = '';
    $scope.password = '';
    $scope.message = '';

    // Hàm đăng nhập
    $scope.login = function() {
        AuthService.login($scope.username, $scope.password)
            .then(function(response) {
                $scope.message = 'Login successful';
                console.log(response.data);
            })
            .catch(function(error) {
                $scope.message = 'Login failed';
                console.error(error);
            });
    };

    // Hàm đăng xuất
    $scope.logout = function() {
        AuthService.logout()
            .then(function(response) {
                $scope.message = 'Logout successful';
                console.log(response.data);
            })
            .catch(function(error) {
                $scope.message = 'Logout failed';
                console.error(error);
            });
    };

    $scope.loadData = function() {
        AuthService.loadData()
            .then(function(response) {
                $scope.message = 'Load Data successful';
                console.log(response.data);
            })
            .catch(function(error) {
                $scope.message = 'Load Data failed';
                console.error(error);
            });
    };
});

app.controller('ChatController', function($scope, SignalRService) {
    $scope.user = '';
    $scope.message = '';
    $scope.messages = []; // Danh sách tin nhắn

    // Kết nối đến SignalR Hub
     SignalRService.connect();

    // Lắng nghe tin nhắn mới
    SignalRService.onMessageReceived(function(user, message) {
        $scope.$apply(function() {
            $scope.messages.push({ user: user, message: message });
        });
    });

    // Gửi tin nhắn
    $scope.sendMessage = function() {

        if ($scope.user && $scope.message) {
            SignalRService.sendMessage($scope.user, $scope.message);
            $scope.messages.push({ user: 'You', message: $scope.message });
            $scope.message = '';
        }
    };
});