
--DELETE FROM plenmejs.content WHERE  id = 'HTMLEditorTemplate'

UPDATE plenmejs.content set content='<script type="text/javascript" src="<your installation path>/tinymce/tinymce.min.js"></script>
<script type="text/javascript">
tinymce.init({
    selector: "textarea",
    theme: "modern",
    plugins: [
        "advlist autolink lists link image charmap print preview hr anchor pagebreak",
        "searchreplace wordcount visualblocks visualchars code fullscreen",
        "insertdatetime media nonbreaking save table contextmenu directionality",
        "emoticons template paste textcolor colorpicker textpattern"
    ],
    toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
    toolbar2: "print preview media | forecolor backcolor emoticons",
    image_advtab: true,
    templates: [
        {title: ''Test template 1'', content: ''Test 1''},
        {title: ''Test template 2'', content: ''Test 2''}
    ]
});
</script>

<form method="post" action="somepage">
    <textarea name="content" style="width:100%"></textarea>' where id = 'HTMLEditorTemplateTest'

--INSERT INTO plenmejs.content (id, content)
--VALUES ('HTMLEditorTemplateTest','<!DOCTYPE html><html><head>  
--<script src="http://code.jquery.com/jquery-1.11.1.min.js"></script>
--<script src="http://tinymce.cachefly.net/4.1/tinymce.min.js"></script>
--<link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css" />
--<script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/js/bootstrap.min.js"></script>
--<script type="text/javascript">

--tinymce.init({
--            selector: "textarea",
--            theme: "modern",
--            style_formats: [
--           { title: ''Bold text'', inline: ''b'' },
--           { title: ''Red text'', inline: ''span'', styles: { color: ''#ff0000'' } },
--           { title: ''Red header'', block: ''h1'', styles: { color: ''#ff0000'' } },
--           { title: ''Example 1'', inline: ''span'', classes: ''example1'' },
--           { title: ''Example 2'', inline: ''span'', classes: ''example2'' },
--           { title: ''Table styles'' },
--           { title: ''Table row 1'', selector: ''tr'', classes: ''tablerow1'' }
--            ],
--            plugins: [
--                "advlist autolink lists link image charmap print preview hr anchor pagebreak",
--                "searchreplace wordcount visualblocks visualchars code fullscreen",
--                "insertdatetime media nonbreaking save table contextmenu directionality",
--                "emoticons paste textcolor colorpicker textpattern"
--            ],
--            toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
--            toolbar2: "print preview media | forecolor backcolor emoticons",
--            image_advtab: true
--          ]
--        });
--    </script>
--</head>
--<body>
--    <form method="post" action="somepage">
--        <textarea name="content" style="width:100%"></textarea>
--    </form>
--</body>
--</html>');