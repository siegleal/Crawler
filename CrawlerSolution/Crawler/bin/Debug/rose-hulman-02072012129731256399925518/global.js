function setMouseover(selector, onPath, offPath) {
  $(selector).mouseover(function() {
    $(this).attr("src", onPath);
  })
  .mouseout(function(){
    $(this).attr("src", offPath);
  });
}