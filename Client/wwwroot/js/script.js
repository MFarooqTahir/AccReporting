﻿export function ShowReportNewPage(data) {
    var file = new Blob([data], { type: 'application/pdf' });
    var fileURL = URL.createObjectURL(file);
    window.open(fileURL);
}