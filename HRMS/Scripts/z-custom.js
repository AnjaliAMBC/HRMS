//alert('ss');

$(document).ready(function () {
    
  // Function to check width and trigger action
  function checkWidth() {
    var width = $('body').width();
    if (width >= 0 && width <= 480) {
        // Trigger your action here
        console.log('Width is between 0px and 480px');
        $('body').removeClass('tablet-view');
        $('body').addClass('mobile-view');
        $(".mobile-view .res-admin-employee-management .res-admin-empman-filter-show #demo span.res-close-btn").click(function () {
            $('.mobile-view .res-admin-employee-management .res-admin-empman-filter-show #demo').removeClass('show');
        });
    }

    if (width >= 481 && width <= 767) {
        // Trigger your action here
        console.log('Width is between 481px and 767');
        $('body').removeClass('mobile-view');
        $('body').addClass('tablet-view');
        $(".tablet-view .res-admin-employee-management .res-admin-empman-filter-show #demo span.res-close-btn").click(function () {
            $('.tablet-view .res-admin-employee-management .res-admin-empman-filter-show #demo').removeClass('show');
        });
    }

    if (width >= 768 && width <= 1024) {
        // Trigger your action here
        console.log('Width is between 768px and 1024');
        $('body').removeClass('mobile-view');
        $('body').addClass('tablet-view');
    }
  }
  
  // Initial check on page load
  checkWidth();
  
  // Check width on window resize
  $(window).resize(function() {
    checkWidth();
  });

    $(document).off('click', '.responsive-tabs i.fa').on('click', '.responsive-tabs i.fa', function (event) {
        $(this).parent().toggleClass('open');
    });

    $(document).off('click', '.responsive-tabs > li a').on('click', '.responsive-tabs > li a', function (event) {
        $('.responsive-tabs > li').removeClass('active');
        $(this).parent().addClass('active');
        $('.responsive-tabs').toggleClass('open');
    });

    $('.sidebar-dropdown > a').click(function () {
        console.log('ss');
        //$(".sidebar-submenu").toggle();
        $(this).next('.sidebar-submenu').toggle(300);
        $(this).find('i').toggleClass('fa-chevron-down fa-chevron-up');
    });

    $('.sidebar-submenu ul li a').click(function (e) {
        // Remove 'sidebaractive' class from all menu items
        $('.sidebar-submenu ul li').removeClass('sidebaractive');

        // Add 'sidebaractive' class to the clicked menu item
        $(this).parent('li').addClass('sidebaractive');
    });

});
