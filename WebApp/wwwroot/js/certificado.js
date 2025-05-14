var vm = new Vue({
    el: "#vCertificado",
    data: {
        loading: false,
        editDto: {
            Id: "", TipoCurso: "", Curso: "",
            ImagemFrente: "", HtmlFrente: "",
            ImagemVerso: "", HtmlVerso: "",
            NomeImagemFrente: "", NomeImagemVerso: "",
            Status: true
        }
    },
    mounted: function () {
        $('.select2').select2({ allowClear: true });

        $('[data-plugin-ios-switch]').each(function () {
            var $this = $(this);
            $this.themePluginIOS7Switch();
        });

        $('#formCertificado').validate({
            highlight: function (label) {
                $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
            },
            success: function (label) {
                $(label).closest('.form-group').removeClass('has-error');
                label.remove();
            },
            errorPlacement: function (error, element) {
                var placement = element.closest('.input-group');
                if (!placement.get(0)) placement = element;
                if (error.text() !== '') placement.after(error);
            }
        });
    },
    methods: {
        Certificado: function (id) {
            var self = this;
            return axios.get("/Certificado/GetCertificadoById/?id=" + id).then(result => {
                var data = result.data;
                self.editDto.Id = data.id;
                self.editDto.TipoCurso = data.tipoCurso;
                self.editDto.Curso = data.curso;
                self.editDto.HtmlFrente = data.htmlFrente;
                self.editDto.HtmlVerso = data.htmlVerso;
                self.editDto.Status = data.status;
                self.editDto.ImagemFrente = "/Certificados/" + data.imagemFrente;
                self.editDto.ImagemVerso = data.imagemVerso ? "/Certificados/" + data.imagemVerso : null;

                if (self.editDto.ImagemFrente) {
                    applyBackgroundImage('summernoteFrente', self.editDto.ImagemFrente);
                }
                if (self.editDto.ImagemVerso) {
                    applyBackgroundImage('summernoteVerso', self.editDto.ImagemVerso);
                }
            }).catch(error => {
                alert("Erro ao carregar certificado: " + error.message);
            });
        }
    }
});

function applyBackgroundImage(editorId, imageUrl) {
    $('#' + editorId).next('.note-editor').find('.note-editable').css({
        'background-image': 'url(' + imageUrl + ')',
        'background-size': 'contain',
        'background-repeat': 'no-repeat',
        'background-position': 'center center'
    });
}

$(document).ready(function () {
    var id = $('#certificadoId').val();

    if (id) {
        vm.Certificado(id);
    }

    function initializeSummernote(elementId) {
        $('#' + elementId).summernote({
            lang: 'pt-BR',
            height: 813,
            width: 1140,
            toolbar: [
                ['style', ['style']],
                ['font', ['bold', 'italic', 'underline', 'clear']],
                ['fontname', ['fontname']],
                ['fontsize', ['fontsize']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['table', ['table']],
                ['insert', ['link', 'picture']],
                ['view', ['fullscreen', 'codeview']]
            ],
            fontSizes: ['8', '9', '10', '11', '12', '14', '18', '24', '36'],
            callbacks: {
                onInit: function () {
                    $(this).next('.note-editor').find('.note-editing-area').css({
                        'width': '1140px',
                        'height': '813px'
                    });
                },
                onEnterFullscreen: function () {
                    const $editor = $(this).next('.note-editor');
                    $editor.find('.note-editable').css({
                        'background-color': 'white',
                        'margin': '20px auto'
                    });
                },
                onExitFullscreen: function () {
                    const $editor = $(this).next('.note-editor');
                    $editor.find('.note-editable').css({ 'margin-top': '0' });
                }
            }
        });
    }

    initializeSummernote('summernoteFrente');
    initializeSummernote('summernoteVerso');

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href");
        if (target === "#tabFrente" && vm.editDto.ImagemFrente) {
            applyBackgroundImage('summernoteFrente', vm.editDto.ImagemFrente);
        } else if (target === "#tabVerso") {
            if (vm.editDto.ImagemVerso) {
                applyBackgroundImage('summernoteVerso', vm.editDto.ImagemVerso);
            } else {
                $('#summernoteVerso').next('.note-editor').find('.note-editable').css('background-image', 'none');
            }
        }
    });

    $("#ddlTipoFomento").change(function () {
        var tipoFomentoId = $(this).val();
        $("#ddlFomento").empty().append('<option value="">Selecionar Fomento</option>');
        if (tipoFomentoId) {
            $.getJSON(`/Certificado/GetFomentosByTipoFomentoId/${tipoFomentoId}`, function (data) {
                $.each(data, function (i, item) {
                    $("#ddlFomento").append(`<option value="${item.id}">${item.titulo}</option>`);
                });
            });
        }
    });

    $('#formCertificado').on('submit', function () {
        var htmlFrente = $('#summernoteFrente').summernote('code');
        var htmlVerso = $('#summernoteVerso').summernote('code');
        $('<input>').attr({ type: 'hidden', name: 'HtmlFrente', value: htmlFrente }).appendTo(this);
        $('<input>').attr({ type: 'hidden', name: 'HtmlVerso', value: htmlVerso }).appendTo(this);
    });
});

var crud = {
    CertificadoModal: function (id) {
        $('input[name="certificadoId"]').val(id);
        $('#mdCertificado').modal('show');
        vm.Certificado(id); // chama a função para carregar dados do certificado
    },
    DeleteModal: function (id) {
        $('input[name="deleteCertificadoId"]').val(id);
        $('#mdDeleteCertificado').modal('show');
    }
};