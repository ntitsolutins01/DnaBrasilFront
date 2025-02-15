var vm = new Vue({
    el: "#vControleMaterialEstoqueSaida",
    data: {
        loading: false,
        editDto: { Id: "", Quantidade: "", Solicitante: "" }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            //mascara dos inputs
            var cargaHoraria = $("#cargaHoraria");
            cargaHoraria.mask('000', { reverse: false });


            //skin checkbox
            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);

                        $this.themePluginIOS7Switch();
                    });
                });
            }

            var formid = $('form')[1].id;

            if (formid === "formEditControleMaterialEstoqueSaida") {

                $("#formEditControleMaterialEstoqueSaida ").validate({
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

            if (formid === "formControleMaterialEstoqueSaida") {

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

                //Açao de seleçao de valor na combo primaria para preencher a combo secundára
                $("#ddlTipoMaterial").change(function () {
                    var tipoMaterialId = $("#ddlTipoMaterial").val();

                    var url = "../Material/GetMateriaisByTipoMaterialId";

                    $.getJSON(url,
                        { id: tipoMaterialId },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Material</option>';
                                $("#ddlMaterial").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlMaterial").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Material',
                                    text: 'Materiais não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                $("#formControleMaterialEstoqueSaida").validate({
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

                //clique de escolha do select
                $("#ddlEstado").change(function () {

                    self.ShowLoad(true, "pFiltro");

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

                    self.ShowLoad(false, "pFiltro");
                });

                //clique de escolha do select
                $("#ddlMunicipio").change(function () {

                    self.ShowLoad(true, "pFiltro");

                    var id = $("#ddlMunicipio").val();

                    var url = "../Localidade/GetLocalidadeByMunicipio?id=" + id;

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
                                    text: 'Localidades não encontradas.',
                                    type: 'warning'
                                });
                            }
                        });

                    self.ShowLoad(false, "pFiltro");
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
        DeleteControleMaterialEstoqueSaida: function (id) {
            var url = "ControleMaterialEstoqueSaida/Delete/" + id;
            $("#deleteControleMaterialEstoqueSaidaHref").prop("href", url);
        },
        EditControleMaterialEstoqueSaida: function (id) {
            var self = this;

            axios.get("ControleMaterialEstoqueSaida/GetControleMaterialEstoqueSaidaById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Quantidade = result.data.quantidade;
                self.editDto.Solicitante = result.data.solicitante;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteControleMaterialEstoqueSaidaId"]').attr('value', id);
        $('#mdDeleteControleMaterialEstoqueSaida').modal('show');
        vm.DeleteControleMaterialEstoqueSaida(id)
    },
    EditModal: function (id) {
        $('input[name="editControleMaterialEstoqueSaidaId"]').attr('value', id);
        $('#mdEditControleMaterialEstoqueSaida').modal('show');
        vm.EditControleMaterialEstoqueSaida(id)
    }
};