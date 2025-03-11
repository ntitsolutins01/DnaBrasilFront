var vm = new Vue({
    el: "#vMaterial",
    data: {
        loading: false,
        editDto: { Id: "", GrupoMaterialId: "", TipoMaterialId: "", LocalidadeId: "", Descricao: "", UnidadeMedida: "", QtdAdquirida: "" }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

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

            if (formid === "formEditMaterial") {

                $("#formEditMaterial ").validate({
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

            if (formid === "formMaterial" || formid === "formPesquisarMaterial") {

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

                $("#ddlGrupoMaterial").change(function () {

                    self.ShowLoad(true, "pFiltro");

                    var url = "../../TipoMaterial/GetTiposMateriaisByGrupoMaterialId";

                    var ddlSource = "#ddlGrupoMaterial";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Tipo Material</option>';
                                $("#ddlTipoMaterial").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlTipoMaterial").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Tipo de Material',
                                    text: data,
                                    type: 'warning'
                                });
                            }
                        });

                    self.ShowLoad(false, "pFiltro");
                });

                $("#formMaterial").validate({
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
        DeleteMaterial: function (id) {
            var url = "Material/Delete/" + id;
            $("#deleteMaterialHref").prop("href", url);
        },
        EditMaterial: function (id) {
            var self = this;

            axios.get("Material/GetMaterialById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Descricao = result.data.descricao;
                self.editDto.UnidadeMedida = result.data.unidadeMedida;
                self.editDto.QtdAdquirida = result.data.qtdAdquirida;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteMaterialId"]').attr('value', id);
        $('#mdDeleteMaterial').modal('show');
        vm.DeleteMaterial(id)
    },
    EditModal: function (id) {
        $('input[name="editMaterialId"]').attr('value', id);
        $('#mdEditMaterial').modal('show');
        vm.EditMaterial(id)
    }
};