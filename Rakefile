# -*- mode:ruby; encoding: utf-8 -*-

require 'bundler/setup'
require 'albacore'
require 'rake/clean'

task default: :run

DXLIB = 'DxLibDotNet3_11b.zip'

file DXLIB do |t|
  system("@powershell -NoProfile -Command \"(new-object net.webclient).DownloadFile('http://dxlib.o.oo7.jp/DxLib/#{DXLIB}', '#{DXLIB}')\"")
end

unzip unzip_dxlib: DXLIB do |unzip|
  unzip.destination = File.dirname(__FILE__)
  unzip.file = DXLIB
end

file 'DxLibDotNet' => :unzip_dxlib

msbuild msbuild: 'DxLibDotNet' do |b|
  b.solution = 'src/Lemonade.sln'
end

task run: :msbuild do |t|
  system 'start src/Lemonade/bin/Debug/Lemonade.exe'
end

task clean: :msbuild_clean

msbuild :msbuild_clean do |b|
  b.solution = 'src/Lemonade.sln'
  b.targets = [:Clean]
end
