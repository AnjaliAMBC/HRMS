//alert('ss');

$(document).ready(function() {
  // Function to check width and trigger action
  function checkWidth() {
    var width = $('body').width();
    if (width >= 0 && width <= 480) {
      // Trigger your action here
      console.log('Width is between 0px and 480px');
	  $('body').addClass('mobile-view');
      }

      if (width >= 481 && width <= 767) {
          // Trigger your action here
          console.log('Width is between 481px and 767');
          $('body').addClass('tablet-view');
      }

      if (width >= 768 && width <= 1024) {
          // Trigger your action here
          console.log('Width is between 768px and 1024');
          $('body').addClass('tablet-view');
      }
  }
  
  // Initial check on page load
  checkWidth();
  
  // Check width on window resize
  $(window).resize(function() {
    checkWidth();
  });
});
