Json to Resume Formats

What?
My resume in a programmer-friendly format, easily transformed in the browser to various 
computer- and human-friendly formats. Hurray for javascript, jQuery and XHTML!

Why?
I wanted a way to show off some skills, and what better way then coding a resume?

How?
The resume was written in a javascript-friendly json file, then I used jquery and (a slightly 
hacked version of) microsoft's jquery.tmpl plugin to transform it to html and text, and a 
jquery.json2xml plugin to render human-readable xml to the browser.

To do:
Use downloadify and jspdf to create a pdf version of my resume.
Use downloadify and jszip to create a docx version of my resume.