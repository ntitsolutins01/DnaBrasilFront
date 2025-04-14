var vm = new Vue({
    el: "#vModuloEad ",
    data: {
        loading: false,
        editDto: { Id: "", Titulo: "", Descricao: "", Status: true }
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

            if (formid === "formEditModuloEad") {

                $("#formEditModuloEad ").validate({
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

            if (formid === "formModuloEad") {

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
                                    text: 'Cursos não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                $("#formModuloEad").validate({
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
        DeleteModuloEad: function (id) {
            var url = "ModuloEad/Delete/" + id;
            $("#deleteModuloEadHref").prop("href", url);
        },
        EditModuloEad: function (id) {
            var self = this;

            axios.get("ModuloEad/GetModuloEadById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Titulo = result.data.titulo;
                self.editDto.Descricao = result.data.descricao;
                self.editDto.Status = result.data.status;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteModuloEadId"]').attr('value', id);
        $('#mdDeleteModuloEad').modal('show');
        vm.DeleteModuloEad(id)
    },
    EditModal: function (id) {
        $('input[name="editModuloEadId"]').attr('value', id);
        $('#mdEditModuloEad').modal('show');
        vm.EditModuloEad(id)
    }
};