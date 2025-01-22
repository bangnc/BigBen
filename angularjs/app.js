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
