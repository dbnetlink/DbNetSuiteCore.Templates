"use strict";
$(() => { var sample = new Sample(); });
class Sample {
    constructor() {
        var selectedApp = $("div.selected-sample").closest("tr");
        var component = selectedApp.data("component");
        $(`tr.${component}-item`).show();
        $(`tr.${component}-title`).find(".menu-title-text").removeClass("more").addClass("less");
        $('tr.menu-title').on("click", (event) => this.selectSection(event));
    }
    selectSection(event) {
        var $tr = $(event.currentTarget);
        if ($tr.find(".menu-title-text").hasClass("less")) {
            $tr.find(".menu-title-text").removeClass("less").addClass("more");
            var component = $tr.data("component");
            $(`tr.${component}-item`).hide();
            return;
        }
        var href = `/samples/${$tr.next().data("page")}`;
        window.location.href = href;
    }
}
