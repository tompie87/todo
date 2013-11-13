//Config
var TodoApp = angular.module("TodoApp", ["ngResource"]).
    config(function($routeProvider) {
        $routeProvider.
            when('/', { controller: ListCtrl, templateUrl: 'list.html' }).
            when('/new', { controller: CreateCrtl, templateUrl: 'details.html' }).
            when('/edit/:editId',{controller: EditCtrl,templateUrl:'details.html'}).
            otherwise({ redirectTo: '/' });
    });

//Factory voor het ophalen van de data.
TodoApp.factory('Todo', function ($resource) {
    return $resource('/api/todo/:id', { id: '@id' }, { update: { method: 'PUT' } });
});

var CreateCrtl = function ($scope, $location, Todo) {
    $scope.action = "Create";
    $scope.save = function () {
        Todo.save($scope.item, function () {
            $location.path('/');
        });
    };
};

var EditCtrl = function ($scope, $routeParams, $location, Todo) {
    $scope.action = "Edit";
    var id = $routeParams.editId;
    $scope.item = Todo.get({ id: id })
    $scope.save = function () {
        Todo.update({ id: id }, $scope.item, function () {
            $location.path('/'); 
        });
    };
};


//Controller zegt welke code je moet runnen, injecteer de Todo van de factory in de controller.  
var ListCtrl = function ($scope, $location, Todo) {
    $scope.search = function () {
        $scope.items = Todo.query({
            q: $scope.query,
            sort: $scope.sort_order, 
            desc: $scope.is_desc,
            limit:$scope.limit,
            offset:$scope.offset,            
        },
        function (data) {
            //$scope.more = data.length === 5;
            //$scope.items = $scope.items.concat(data); //Werkt nog niet goed. Hoe ga je terug naar de vorige set?
        });
    };

    //Functie gaat de data krijgen die uit de query komt. 
    $scope.sort = function (col) {
        if ($scope.sort_order === col)
        {
            $scope.is_desc = !$scope.is_desc;
        }
        else {
            $scope.sort_order = col;
            $scope.is_desc = false; 
        }
        $scope.reset();
    };

    $scope.has_more = function () {
        return $scope.more;
    }

    $scope.show_more = function () {
        debugger;
        $scope.offset += $scope.limit;
        $scope.search();
    }

    $scope.reset = function () {
        $scope.limit = 5;
        $scope.offset = 0;
        $scope.items = [];
        $scope.more = true;
        $scope.search();
    }
    
    //Wordt niet juist naar API doorgegeven.
    $scope.delete = function () {
        var toDoId = this.item.TodoItemId;
        Todo.remove({ id: toDoId }, function () {
            $('#item_' + toDoId).fadeOut();
        }
            );
    }

    $scope.sort_order = 'Priority'
    $scope.is_desc = false;
    $scope.limit = 5;
    $scope.items = [];
    $scope.more = true;
    $scope.search();
};