(function () {

    'use strict';
    //mascara dos inputs
    var $numCpf = $("#cpf");
    $numCpf.mask('000.000.000-00', { reverse: false });

    //skin select
    var $select = $(".select2").select2({
        allowClear: true
    });

    $(".select2").each(function () {
        var $this = $(this),
            opts = {};

        var pluginOptions = $this.data('plugin-options');
        if (pluginOptions)
            opts = pluginOptions;

        $this.themePluginSelect2(opts);
    });

    /*
     * When you change the value the select via select2, it triggers
     * a 'change' event, but the jquery validation plugin
     * only re-validates on 'blur'*/

    $select.on('change', function () {
        $(this).trigger('blur');
    });

    jQuery.validator.addMethod("cpf", function (cpf, element) {
        var regex = /^\d{3}\.\d{3}\.\d{3}\-\d{2}$/;
        var add, rev, i;
        if (!regex.test(cpf))
            return false;

        cpf = cpf.replace(/[^\d]+/g, '');
        if (cpf == '') return false;
        // Elimina CPFs invalidos conhecidos	
        if (cpf.length != 11 ||
            cpf == "11111111111" ||
            cpf == "22222222222" ||
            cpf == "33333333333" ||
            cpf == "44444444444" ||
            cpf == "55555555555" ||
            cpf == "66666666666" ||
            cpf == "77777777777" ||
            cpf == "88888888888" ||
            cpf == "99999999999")
            return false;
        // Valida 1o digito	
        add = 0;
        for (i = 0; i < 9; i++)
            add += parseInt(cpf.charAt(i)) * (10 - i);
        rev = 11 - (add % 11);
        if (rev == 10 || rev == 11)
            rev = 0;
        if (rev != parseInt(cpf.charAt(9)))
            return false;
        // Valida 2o digito	
        add = 0;
        for (i = 0; i < 10; i++)
            add += parseInt(cpf.charAt(i)) * (11 - i);
        rev = 11 - (add % 11);
        if (rev == 10 || rev == 11)
            rev = 0;
        if (rev != parseInt(cpf.charAt(10)))
            return false;
        return true;


    }, "Informe um CPF válido");

    //clique de escolha do select
    $("#ddlEstado").change(function () {
        var sigla = $("#ddlEstado").val();

        var url = "../../DivisaoAdministrativa/GetMunicipioByUf?uf=" + sigla;

        var ddlSource = "#ddlMunicipio";

        $.getJSON(url,
            { id: $(ddlSource).val() },
            function (data) {
                if (data.length > 0) {
                    var items = '<option value="">Selecionar Municipio</option>';
                    $("#ddlMunicipio").empty;
                    $.each(data,
                        function (i, row) {
                            items += "<option value='" + row.value + "'>" + row.text + "</option>";
                        });
                    $("#ddlMunicipio").html(items);
                }
                else {
                    new PNotify({
                        title: 'Usuario',
                        text: data,
                        type: 'warning'
                    });
                }
            });
    });



    // basic
    $("#form").validate({
        rules: {
            //"Login.Email": {
            //    required: true,
            //    email: true
            //},
            "Login.Password": {
                required: true,
                minlength: 8
            },
            "Input.Password": {
                required: true,
                minlength: 8
            },
            "Input.ConfirmPassword": {
                required: true,
                minlength: 8,
                //equalTo: "#Input.Password"
            },
            //"Input.Email": {
            //    required: true
            //},
            cpf: { cpf: true, required: true }
        },
        messages: {
            //"Login.Email": {
            //    required: "Por favor informe seu usuário.",
            //    email: "Formato de e-mail inválido."
            //},
            "Login.Password": {
                required: "Por favor informe sua senha.",
                minlength: jQuery.validator.format("Formato de senha inválido, a senha deve conter no mínimo 8 digitos.")
            },
            "Input.Password": {
                required: "Por favor informe sua senha.",
                minlength: jQuery.validator.format("Formato de senha inválido, a senha deve conter no mínimo 8 digitos.")
            },
            "Input.ConfirmPassword": {
                required: "Por favor informe sua senha.",
                minlength: jQuery.validator.format("Formato de senha inválido, a senha deve conter no mínimo 8 digitos."),
                //equalTo: "As senhas digitadas são diferentes. Por favor, repita a operação."
            },
            //"Input.Email": {
            //    required: "Por favor informe seu usuário.",
            //},
            cpf: { cpf: 'Formato de CPF inválido', required: "Por favor informe o número do CPF." }
        },
        highlight: function (label) {
            $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
        },
        success: function (label) {
            $(label).closest('.form-group').removeClass('has-error');
            label.remove();
        },
        errorPlacement: function (error, element) {
            var placement = element.closest('.input-group');
            if (!placement.get(0)) {
                placement = element;
            }
            if (error.text() !== '') {
                placement.after(error);
            }
        }
    });

}).apply(this, [jQuery]);