var vm = new Vue({
    el: "#vUsuario",
    data: {
        params: {
            cpf: "",
            email: "",
            perfil: "",
            visible: true
        },
        loading: false
    },
    mounted: function () {
        var self = this;
        (function ($) {

            'use strict';
            var formid = $('form')[1].id;

            if (formid === "formUsuario") {
                if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                    $(function () {
                        $('[data-plugin-ios-switch]').each(function () {
                            var $this = $(this);

                            $this.themePluginIOS7Switch();
                        });
                    });
                }

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
                //clique de escolha do select
                $("#ddlEstado").change(function () {
                    var sigla = $("#ddlEstado").val();

                    var url = "../DivisaoAdministrativa/GetMunicipioByUf?uf=" + sigla;

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

                //clique de escolha do select
                $("#ddlMunicipio").change(function () {
                    var id = $("#ddlMunicipio").val();

                    var url = "../../Localidade/GetLocalidadeByMunicipio?id=" + id;

                    var ddlSource = "#ddlLocalidade";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Localidade</option>';
                                $("#ddlLocalidade").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlLocalidade").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Localidades',
                                    text: data,
                                    type: 'warning'
                                });
                            }
                        });
                });

                var $numCpf = $("#cpf");
                $numCpf.mask('000.000.000-00', { reverse: false });
                jQuery.validator.addMethod("cpf", function (cpf, element) {
                    var regex = /^\d{3}\.\d{3}\.\d{3}\-\d{2}$/;
                    var add, rev, i;
                    if (!regex.test(cpf))
                        return false;

                    cpf = cpf.replace(/[^\d]+/g, '');
                    if (cpf == '') return false;
                    // Elimina CPFs invalidos conhecidos	
                    if (cpf.length != 11 ||
                        cpf == "00000000000" ||
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

                var $numCnpj = $("#cnpj");
                $numCnpj.mask('00.000.000/0000-00', { reverse: false });

                jQuery.validator.addMethod("cnpj",
                    function (value, element) {

                        var numeros, digitos, soma, i, resultado, pos, tamanho, digitos_iguais;
                        if (value.length == 0) {
                            return false;
                        }

                        value = value.replace(/\D+/g, '');
                        digitos_iguais = 1;

                        for (i = 0; i < value.length - 1; i++)
                            if (value.charAt(i) != value.charAt(i + 1)) {
                                digitos_iguais = 0;
                                break;
                            }
                        if (digitos_iguais)
                            return false;

                        tamanho = value.length - 2;
                        numeros = value.substring(0, tamanho);
                        digitos = value.substring(tamanho);
                        soma = 0;
                        pos = tamanho - 7;
                        for (i = tamanho; i >= 1; i--) {
                            soma += numeros.charAt(tamanho - i) * pos--;
                            if (pos < 2)
                                pos = 9;
                        }
                        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                        if (resultado != digitos.charAt(0)) {
                            return false;
                        }
                        tamanho = tamanho + 1;
                        numeros = value.substring(0, tamanho);
                        soma = 0;
                        pos = tamanho - 7;
                        for (i = tamanho; i >= 1; i--) {
                            soma += numeros.charAt(tamanho - i) * pos--;
                            if (pos < 2)
                                pos = 9;
                        }

                        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;

                        return (resultado == digitos.charAt(1));
                    },
                    "Informe um CNPJ válido");


                var $tipoPessoa = $("input:radio[name=tipoPessoa]");
                $tipoPessoa.on("change", function () {

                    if ($(this).val() == "pf") {
                        self.params.visible = true;
                    } else if ($(this).val() == "pj") {

                        var $numCnpj = $("#cnpj");
                        $numCnpj.mask('00.000.000/0000-00', { reverse: false });

                        self.params.visible = false;

                    }
                });

                if (formid === "formUsuario") {
                    $("#formUsuario").validate({
                        rules: {
                            "email": {
                                required: true,
                                email: true
                            },
                            cpf: { cpf: true, required: true },
                            cnpj: { cnpj: true, required: true }
                        },
                        messages: {
                            "email": {
                                required: "Por favor informe o endereço eletrônico válido do usuário.",
                                email: "Formato de e-mail inválido."
                            },
                            cpf: { cpf: 'Formato de CPF inválido', required: "Por favor informe o número do CPF do parceiro." },
                            cnpj: { cnpj: 'Formato de CNPJ inválido', required: "Por favor informe o número do CNPJ do parceiro." }
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
                }

                if (formid === "formEditUsuario") {
                    $("#formUsuario").validate({
                        rules: {
                            "email": {
                                required: true,
                                email: true
                            },
                            cpf: { cpf: true, required: true },
                            cnpj: { cnpj: true, required: true }
                        },
                        messages: {
                            "email": {
                                required: "Por favor informe o endereço eletrônico válido do usuário.",
                                email: "Formato de e-mail inválido."
                            },
                            cpf: { cpf: 'Formato de CPF inválido', required: "Por favor informe o número do CPF do parceiro." },
                            cnpj: { cnpj: 'Formato de CNPJ inválido', required: "Por favor informe o número do CNPJ do parceiro." }
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
                }

                
            }

            

            //Ediçao
            if (formid === "formEditUsuario") {

                if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                    $(function () {
                        $('[data-plugin-ios-switch]').each(function () {
                            var $this = $(this);

                            $this.themePluginIOS7Switch();
                        });
                    });
                }

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

                //clique de escolha do select
                $("#ddlMunicipio").change(function () {
                    var id = $("#ddlMunicipio").val();

                    var url = "../../Localidade/GetLocalidadeByMunicipio?id=" + id;

                    var ddlSource = "#ddlLocalidade";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Localidade</option>';
                                $("#ddlLocalidade").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlLocalidade").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Localidades',
                                    text: data,
                                    type: 'warning'
                                });
                            }
                        });
                });

                var $numCpf = $("#cpf");
                $numCpf.mask('000.000.000-00', { reverse: false });
                jQuery.validator.addMethod("cpf", function (cpf, element) {
                    var regex = /^\d{3}\.\d{3}\.\d{3}\-\d{2}$/;
                    var add, rev, i;
                    if (!regex.test(cpf))
                        return false;

                    cpf = cpf.replace(/[^\d]+/g, '');
                    if (cpf == '') return false;
                    // Elimina CPFs invalidos conhecidos	
                    if (cpf.length != 11 ||
                        cpf == "00000000000" ||
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

                var $numCnpj = $("#cnpj");
                $numCnpj.mask('00.000.000/0000-00', { reverse: false });

                jQuery.validator.addMethod("cnpj",
                    function (value, element) {

                        var numeros, digitos, soma, i, resultado, pos, tamanho, digitos_iguais;
                        if (value.length == 0) {
                            return false;
                        }

                        value = value.replace(/\D+/g, '');
                        digitos_iguais = 1;

                        for (i = 0; i < value.length - 1; i++)
                            if (value.charAt(i) != value.charAt(i + 1)) {
                                digitos_iguais = 0;
                                break;
                            }
                        if (digitos_iguais)
                            return false;

                        tamanho = value.length - 2;
                        numeros = value.substring(0, tamanho);
                        digitos = value.substring(tamanho);
                        soma = 0;
                        pos = tamanho - 7;
                        for (i = tamanho; i >= 1; i--) {
                            soma += numeros.charAt(tamanho - i) * pos--;
                            if (pos < 2)
                                pos = 9;
                        }
                        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                        if (resultado != digitos.charAt(0)) {
                            return false;
                        }
                        tamanho = tamanho + 1;
                        numeros = value.substring(0, tamanho);
                        soma = 0;
                        pos = tamanho - 7;
                        for (i = tamanho; i >= 1; i--) {
                            soma += numeros.charAt(tamanho - i) * pos--;
                            if (pos < 2)
                                pos = 9;
                        }

                        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;

                        return (resultado == digitos.charAt(1));
                    },
                    "Informe um CNPJ válido");


                var $tipoPessoa = $("input:radio[name=tipoPessoa]");
                $tipoPessoa.on("change", function () {

                    if ($(this).val() == "pf") {
                        self.params.visible = true;
                    } else if ($(this).val() == "pj") {

                        var $numCnpj = $("#cnpj");
                        $numCnpj.mask('00.000.000/0000-00', { reverse: false });

                        self.params.visible = false;

                    }
                });
                 
                $("#formEditUsuario").validate({
                    rules: {
                        "email": {
                            required: true,
                            email: true
                        },
                        cpf: { cpf: true, required: true },
                        cnpj: { cnpj: true, required: true }
                    },
                    messages: {
                        "email": {
                            required: "Por favor informe o endereço eletrônico válido do usuário.",
                            email: "Formato de e-mail inválido."
                        },
                        cpf: { cpf: 'Formato de CPF inválido', required: "Por favor informe o número do CPF do parceiro." },
                        cnpj: { cnpj: 'Formato de CNPJ inválido', required: "Por favor informe o número do CNPJ do parceiro." }
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
            }
        }).apply(this, [jQuery]);
    },
    methods: {
        ShowLoad: function (flag, el) {
            var self = this;

            self.isLoading = flag;
            $("#" + el).loadingOverlay({
                "startShowing": flag
            });
            self.loading = flag;

            if (!flag) {
                self.isLoading = flag;
                $("#" + el).removeClass("loading-overlay-showing");
                self.loading = flag;
            } else {
                self.isLoading = flag;
                $("#" + el).addClass("loading-overlay-showing");
                self.loading = flag;
            }
        },
        ExisteCpf: function () {
            var self = this;
            self.ShowLoad(true, "pUsuario");

            axios.get("GetUsuarioByCpf/?cpf=" + self.params.cpf).then(result => {

                if (result.data === false) {
                   Site.Notification("Usuário", "Já existe um usuário cadastrado com esse cpf.", "warning", 2);
                } else if (result.data !== true) {
                    Site.Notification("Usuário", result.data, "warning", 2);
                    self.ShowLoad(false, "vUsuario");
                }

                self.ShowLoad(false, "vUsuario");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.response.data, "error", 2);
                self.ShowLoad(false, "vUsuario");
            });
        },
        ExisteEmail: function () {
            var self = this;
            self.ShowLoad(true, "pUsuario");

            axios.get("GetUsuarioByEmail/?email=" + self.params.email).then(result => {

                if (result.data === false) {
                    Site.Notification("Usuário", "Já existe um usuário cadastrado com esse email.", "warning", 2);
                } else if (result.data !== true) {
                    Site.Notification("Usuário", result.data, "warning", 2);
                    self.ShowLoad(false, "vUsuario");
                }
                self.ShowLoad(false, "vUsuario");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.response.data, "error", 2);
                self.ShowLoad(false, "vUsuario");
            });
        },
        DeleteUsuario: function (id) {
            var url = "Usuario/Delete/" + id;
            $("#deleteUsuarioHref").prop("href", url);
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="usuarioId"]').attr('value', id);
        $('#mdDeleteUsuario').modal('show');
        vm.DeleteUsuario(id)
    }
};