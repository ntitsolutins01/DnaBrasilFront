var vm = new Vue({
    el: "#vTipoMaterial",
    data: {
        loading: false,
        editDto: { Id: "", Nome: "" }
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

            if (formid === "formEditTipoMaterial") {

                $("#formEditTipoMaterial ").validate({
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

            if (formid === "formTipoMaterial") {

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
                $("#ddlTipoCurso").change(function () {
                    var tipoCursoId = $("#ddlTipoCurso").val();

                    var url = "../Curso/GetCursosAllByTipoCursoId";

                    var ddlSource = "#ddlCurso";

                    $.getJSON(url,
                        { id: tipoCursoId },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Curso</option>';
                                $("#ddlCurso").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlCurso").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Curso',
                                    text: 'Curso não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                $("#formTipoMaterial").validate({
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
        DeleteTipoMaterial: function (id) {
            var url = "TipoMaterial/Delete/" + id;
            $("#deleteTipoMaterialHref").prop("href", url);
        },
        EditTipoMaterial: function (id) {
            var self = this;

            axios.get("TipoMaterial/GetTipoMaterialById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Nome = result.data.nome;
              
            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteTipoMaterialId"]').attr('value', id);
        $('#mdDeleteTipoMaterial').modal('show');
        vm.DeleteTipoMaterial(id)
    },
    EditModal: function (id) {
        $('input[name="editTipoMaterialId"]').attr('value', id);
        $('#mdEditTipoMaterial').modal('show');
        vm.EditTipoMaterial(id)
    }
};